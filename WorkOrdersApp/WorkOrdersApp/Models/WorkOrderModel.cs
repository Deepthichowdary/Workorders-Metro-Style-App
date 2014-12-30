using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WorkOrdersApp.Models
{
    // WorkOrderModel is a static class to store the data list from JSON
    public partial class WorkOrderModel 
    {
        
       public Dictionary<string, string> Attributes { get; set; }
       public AssignedBy Assigned_By__r = new AssignedBy();
       public Webpage Webpage__r = new Webpage();
       public String Name { get; set; }
       public String Customer_Name__c { get; set; }
       public String Assigned_By__c { get; set; }
       public String Assigned_To__c { get; set; }
       public String Address__c { get; set; }
       public String State__c { get; set; }
       public String Country__c { get; set; }
       public String City__c { get; set; }
       public String Id { get; set; }
       public String Work_Status__c { get; set; }
       public DateTime CreatedDate { get; set; }
       public String Phone__c { get; set; }
       public String Product_Details__c { get; set; }
       public String Product_Name__c { get; set; }
       public String Suspend_Reason__c { get; set; }
       public DateTime WorkOrder_End_Date__c { get; set; }
       public String Zip_Code__c { get; set; }
       public String ProblemOptions__c { get; set; }
       public String CustomerAvailability__c { get; set; }
       public String IsProductReplaced__c { get; set; }
       public String Comments__c { get; set; }
       
       

    }
    public class AssignedBy
    {
        public Dictionary<string, string> Attributes { get; set; }
        public String Name { get; set; }
        public String Id { get; set; }
    }

    
    public class Webpage
    {
        public Dictionary<string, string> Attributes { get; set; }
        public String HTML__c { get; set; }
        public String Id { get; set; }
    }
    public static class WorkOrderList
    {
       public static List<WorkOrderModel> WorkOrdersList = new List<WorkOrderModel>();

    }

    // To store the changes done to the current working order
    public static class CurrentWorkOrder
    {

        public static WorkOrderModel CurretntWorkOrderModelObj = new WorkOrderModel();


    }
    

}
