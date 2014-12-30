using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkOrdersApp.Models
{
    public class WorkOrder
    {
        // Fields in local SQLite DB
       [SQLite.PrimaryKey]
        public String Id { get; set; }
       public String Work_Status__c { get; set; }
       public String Suspend_Reason__c { get; set; }
       public String WorkOrder_End_Date__c { get; set; }
       public String ProblemOptions__c { get; set; }
       public String CustomerAvailability__c { get; set; }
       public String IsProductReplaced__c { get; set; }
       public String Comments__c { get; set; }
    }
}
