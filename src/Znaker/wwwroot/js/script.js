$(document).ready(function() {
   $("#searchForm").on("submit", function() {
       this.action += $(this).find("input").val();
   });
});