using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PostgreSqlProvider;
using PostgreSqlProvider.Entities;

namespace MyToPg
{
    public class Transfer
    {
        private readonly DbContextOptionsBuilder<ZnakerContext> _pgBuilder;
        private readonly DbContextOptionsBuilder _myBuilder;

        private PostgresContext PgContext => new PostgresContext(_pgBuilder.Options);
        private MysqlContext MyContext => new MysqlContext(_myBuilder.Options);

        private readonly ConcurrentQueue<PhoneItem> _jobList = new ConcurrentQueue<PhoneItem>();

        private bool _loading = true;
        private int _totalItems;
        private int _itemsLoaded;
        private int _itemsSkipped;
        private int _itemsAdded;

        public Transfer()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            _pgBuilder = new DbContextOptionsBuilder<ZnakerContext>();
            _pgBuilder.UseNpgsql(configuration["PgConnectionString"], b => b.CommandTimeout(100000));

            _myBuilder = new DbContextOptionsBuilder();
            _myBuilder.UseMySql(configuration["MyConnectionString"], b => b.CommandTimeout(100000));

            using (var context = MyContext)
            {
                _totalItems = context.Phones.Count();
            }
            using (var context = PgContext)
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE public.\"Contacts\", public.\"Entries\", public.\"EntryContacts\" RESTART IDENTITY;");
                context.SaveChanges();
            }
        }

        public void Run()
        {
            Task.Factory.StartNew(Stats);
            Task.Factory.StartNew(LoadData);
            Task.WaitAll(
                    SaveData(), SaveData(), SaveData(),SaveData(), SaveData(), SaveData(),
                    SaveData(), SaveData(), SaveData(),SaveData(), SaveData(), SaveData(),
                    SaveData(), SaveData(), SaveData(),SaveData(), SaveData(), SaveData(),
                    SaveData(), SaveData(), SaveData(),SaveData(), SaveData(), SaveData()
                );
        }

        private async Task Stats()
        {
            while (_loading || _jobList.Any())
            {
                Console.WriteLine(
                    $"{DateTime.Now}: loaded: {_itemsLoaded} of {_totalItems}, skipped: {_itemsSkipped}, added: {_itemsAdded}");
                await Task.Delay(1000);
            }
        }
        private async Task LoadData()
        {
            using (var context = MyContext)
            {
                await context.Phones.AsNoTracking().Include(c => c.PhoneInfos).ForEachAsync(phone =>
                {
                    _itemsLoaded++;
                    var item = new PhoneItem
                    {
                        Number = phone.Number.ToString()
                    };
                    if (item.Number.HasText() == false)
                    {
                        _itemsSkipped++;
                        return;
                    }
                    if (phone.UpdatedAt.HasValue)
                    {
                        item.CreatedAt = phone.UpdatedAt.Value;
                    }
                    else
                    {
                        item.CreatedAt = phone.CreatedAt ?? DateTime.Now;
                    }
                    item.DataItems = new List<DataItem>();

                    foreach (var source in phone.PhoneInfos.Where(z => Regex.IsMatch(z.SourceId, "avito|olx")))
                    {
                        JToken json;
                        try
                        {
                            if (JObject.Parse(source.Data).TryGetValue("description", out json) == false || json.Type != JTokenType.String || json.Value<string>().HasText() == false)
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        var dataItem = new DataItem
                        {
                            IdAtSource = source.IdOnSource,
                            Data = json.Value<string>(),
                            Source = source.SourceId.Contains("olx") ? SourceType.OlxUa : SourceType.Avito
                        };
                        if (source.UpdatedAt.HasValue)
                        {
                            dataItem.CreatedAt = source.UpdatedAt.Value;
                        }
                        else
                        {
                            dataItem.CreatedAt = source.CreatedAt ?? DateTime.Now;
                        }
                        item.DataItems.Add(dataItem);
                    }
                    if (item.DataItems.Any() == false)
                    {
                        _itemsSkipped++;
                        return;
                    }
                    _jobList.Enqueue(item);
                });
            }
            _loading = false;
        }
        private async Task SaveData()
        {
            while (_loading || _jobList.Any())
            {
                PhoneItem item;
                if (_jobList.TryDequeue(out item))
                {
                    using (var context = PgContext)
                    {
                        var contact = new Contact
                        {
                            ContactType = ContactType.Phone,
                            CreatedOn = item.CreatedAt,
                            Identity = item.Number,
                            EntryContacts = new List<EntryContact>(),
                            UpdatedOn = item.CreatedAt
                        };
                        context.Contacts.Add(contact);
                        foreach (var data in item.DataItems)
                        {
                            contact.EntryContacts.Add(new EntryContact
                            {
                                Entry = new Entry
                                {
                                    CreatedOn = data.CreatedAt,
                                    IdOnSource = data.IdAtSource,
                                    SourceId = (int) data.Source,
                                    Text = data.Data
                                }
                            });
                        }
                        await context.SaveChangesAsync();
                        _itemsAdded++;
                    }    
                }

                await Task.Delay(100);
            }
        }
    }
}
