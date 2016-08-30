DO $$
BEGIN
    IF NOT EXISTS(
        SELECT schema_name
          FROM information_schema.schemata
          WHERE schema_name = 'hangfire'
      )
    THEN
      EXECUTE 'CREATE SCHEMA "hangfire";';
    END IF;

END
$$;

SET search_path = 'hangfire';
--
-- Table structure for table `Schema`
--

CREATE TABLE IF NOT EXISTS "schema" (  "version" INT NOT NULL ,
  PRIMARY KEY ("version")
); 


DO
$$
BEGIN
    IF EXISTS (SELECT 1 FROM "schema" WHERE "version"::integer >= 3) THEN
        RAISE EXCEPTION 'version-already-applied';
    END IF;
END
$$;

INSERT INTO "schema"("version") values('1');

--
-- Table structure for table `Counter`
--

CREATE TABLE IF NOT EXISTS "counter" (  "id" SERIAL NOT NULL ,
  "key" VARCHAR(100) NOT NULL ,
  "value" SMALLINT NOT NULL ,
  "expireat" TIMESTAMP NULL ,
  PRIMARY KEY ("id")
); 

DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_counter_key" ON "counter" ("key");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX ix_hangfire_counter_key already exists.';
    END;
END;
$$;

--
-- Table structure for table `Hash`
--

CREATE TABLE IF NOT EXISTS "hash" (  "id" SERIAL NOT NULL ,
  "key" VARCHAR(100) NOT NULL ,
  "field" VARCHAR(100) NOT NULL ,
  "value" TEXT NULL ,
  "expireat" TIMESTAMP NULL ,
  PRIMARY KEY ("id"),
  UNIQUE ("key","field")
); 


--
-- Table structure for table `Job`
--

CREATE TABLE IF NOT EXISTS "job" (  "id" SERIAL NOT NULL ,
  "stateid" INT NULL ,
  "statename" VARCHAR(20) NULL ,
  "invocationdata" TEXT NOT NULL ,
  "arguments" TEXT NOT NULL ,
  "createdat" TIMESTAMP NOT NULL ,
  "expireat" TIMESTAMP NULL ,
  PRIMARY KEY ("id")
); 

DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_job_statename" ON "job" ("statename");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX "ix_hangfire_job_statename" already exists.';
    END;
END;
$$;

--
-- Table structure for table `State`
--

CREATE TABLE IF NOT EXISTS "state" (  "id" SERIAL NOT NULL ,
  "jobid" INT NOT NULL ,
  "name" VARCHAR(20) NOT NULL ,
  "reason" VARCHAR(100) NULL ,
  "createdat" TIMESTAMP NOT NULL ,
  "data" TEXT NULL ,
  PRIMARY KEY ("id"),FOREIGN KEY ("jobid") REFERENCES "job" ( "id" ) ON UPDATE CASCADE ON DELETE CASCADE
); 

DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_state_jobid" ON "state" ("jobid");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX "ix_hangfire_state_jobid" already exists.';
    END;
END;
$$;




--
-- Table structure for table `JobQueue`
--

CREATE TABLE IF NOT EXISTS "jobqueue" (  "id" SERIAL NOT NULL ,
  "jobid" INT NOT NULL ,
  "queue" VARCHAR(20) NOT NULL ,
  "fetchedat" TIMESTAMP NULL ,
  PRIMARY KEY ("id")
); 

DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_jobqueue_queueandfetchedat" ON "jobqueue" ("queue","fetchedat");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX "ix_hangfire_jobqueue_queueandfetchedat" already exists.';
    END;
END;
$$;


--
-- Table structure for table `List`
--

CREATE TABLE IF NOT EXISTS "list" (  "id" SERIAL NOT NULL ,
  "key" VARCHAR(100) NOT NULL ,
  "value" TEXT NULL ,
  "expireat" TIMESTAMP NULL ,
  PRIMARY KEY ("id")
); 


--
-- Table structure for table `Server`
--

CREATE TABLE IF NOT EXISTS "server" (  "id" VARCHAR(50) NOT NULL ,
  "data" TEXT NULL ,
  "lastheartbeat" TIMESTAMP NOT NULL ,
  PRIMARY KEY ("id")
); 


--
-- Table structure for table `Set`
--

CREATE TABLE IF NOT EXISTS "set" (  "id" SERIAL NOT NULL ,
  "key" VARCHAR(100) NOT NULL ,
  "score" FLOAT8 NOT NULL ,
  "value" TEXT NOT NULL ,
  "expireat" TIMESTAMP NULL ,
  PRIMARY KEY ("id"),
  UNIQUE ("key","value")
); 


--
-- Table structure for table `JobParameter`
--

CREATE TABLE IF NOT EXISTS "jobparameter" (  "id" SERIAL NOT NULL ,
  "jobid" INT NOT NULL ,
  "name" VARCHAR(40) NOT NULL ,
  "value" TEXT NULL ,
  PRIMARY KEY ("id"),FOREIGN KEY ("jobid") REFERENCES "job" ( "id" ) ON UPDATE CASCADE ON DELETE CASCADE
); 

DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_jobparameter_jobidandname" ON "jobparameter" ("jobid","name");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX "ix_hangfire_jobparameter_jobidandname" already exists.';
    END;
END;
$$;

CREATE TABLE IF NOT EXISTS "lock" ( "resource" VARCHAR(100) NOT NULL ,
  UNIQUE ("resource")
); 



SET search_path = 'hangfire';
--
-- Table structure for table `Schema`
--

DO
$$
BEGIN
    IF EXISTS (SELECT 1 FROM "schema" WHERE "version"::integer >= 4) THEN
        RAISE EXCEPTION 'version-already-applied';
    END IF;
END
$$;

ALTER TABLE "counter" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "lock" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "hash" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "job" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "jobparameter" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "jobqueue" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "list" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "server" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "set" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;
ALTER TABLE "state" ADD COLUMN "updatecount" integer NOT NULL DEFAULT 0;


SET search_path = 'hangfire';
--
-- Table structure for table `Schema`
--

DO
$$
BEGIN
    IF EXISTS (SELECT 1 FROM "schema" WHERE "version"::integer >= 5) THEN
        RAISE EXCEPTION 'version-already-applied';
    END IF;
END
$$;

ALTER TABLE "server" ALTER COLUMN "id" TYPE VARCHAR(100);


SET search_path = 'hangfire';
--
-- Adds indices, greatly speeds-up deleting old jobs.
--

DO
$$
BEGIN
    IF EXISTS (SELECT 1 FROM "schema" WHERE "version"::integer >= 6) THEN
        RAISE EXCEPTION 'version-already-applied';
    END IF;
END
$$;


DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_counter_expireat" ON "counter" ("expireat");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX ix_hangfire_counter_expireat already exists.';
    END;
END;
$$;

DO $$
BEGIN
    BEGIN
        CREATE INDEX "ix_hangfire_jobqueue_jobidandqueue" ON "jobqueue" ("jobid","queue");
    EXCEPTION
        WHEN duplicate_table THEN RAISE NOTICE 'INDEX "ix_hangfire_jobqueue_jobidandqueue" already exists.';
    END;
END;
$$;

