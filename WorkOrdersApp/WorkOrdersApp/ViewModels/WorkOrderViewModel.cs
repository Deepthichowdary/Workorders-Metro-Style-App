using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using WorkOrdersApp.Models;

// WorkOrderViewModel contains the SQL statements for querying SQLite
namespace WorkOrdersApp.ViewModels
{
    public class WorkOrderViewModel : ViewModelBase
    {
        #region Properties

        private string id = string.Empty;
        public string Id
        {
            get
            { return id; }

            set
            {
                if (id == value)
                { return; }

                id = value;
                RaisePropertyChanged("Id");
            }
        }

        private string status = string.Empty;
        public string Work_Status__c
        {
            get
            { return status; }

            set
            {
                if (status == value)
                { return; }

                status = value;
                RaisePropertyChanged("Work_Status__c");
            }
        }


        private string reason = string.Empty;
        public string Suspend_Reason__c
        {
            get
            { return reason; }

            set
            {
                if (reason == value)
                { return; }

                reason = value;
                RaisePropertyChanged("Suspend_Reason__c");
            }
        }

        private string endDate;
        public string WorkOrder_End_Date__c
        {
            get
            { return endDate; }

            set
            {
                if (endDate == value)
                { return; }

                endDate = value;
                RaisePropertyChanged("WorkOrder_End_Date__c");
            }
        }

        private String options;
        public String ProblemOptions__c
        {
            get
            { return options; }

            set
            {
                if (options == value)
                { return; }

                options = value;
                RaisePropertyChanged("ProblemOptions__c");
            }
        }

        private String availability;
        public String CustomerAvailability__c
        {
            get
            { return availability; }

            set
            {
                if (availability == value)
                { return; }

                availability = value;
                RaisePropertyChanged("CustomerAvailability__c");
            }
        }

        private String comments;
        public String Comments__c
        {
            get
            { return comments; }

            set
            {
                if (comments == value)
                { return; }

                comments = value;
                RaisePropertyChanged("Comments__c");
            }
        }

        private String replaced;
        public String IsProductReplaced__c
        {
            get
            { return replaced; }

            set
            {
                if (replaced == value)
                { return; }

                replaced = value;
                RaisePropertyChanged("IsProductReplaced__c");
            }
        }
        #endregion "Properties"

        // Get the workorder using its work order Id
        public WorkOrderViewModel GetWorkOrder(string workOrderId)
        {
            var workorder = new WorkOrderViewModel();
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                var _workOrder = (db.Table<WorkOrder>().Where(
                    c => c.Id.Equals(workOrderId))).Single();
                workorder.Id = _workOrder.Id;
                workorder.reason = _workOrder.Suspend_Reason__c;
                workorder.status = _workOrder.Work_Status__c;
                workorder.endDate = _workOrder.WorkOrder_End_Date__c;
                workorder.Comments__c = _workOrder.Comments__c;
                workorder.CustomerAvailability__c = _workOrder.CustomerAvailability__c;
                workorder.IsProductReplaced__c = _workOrder.IsProductReplaced__c;
                workorder.ProblemOptions__c = _workOrder.ProblemOptions__c;
            }
            return workorder;
        }

        // Get all the work orders in SQLite
        public List < WorkOrder> GetAllWorkOrders()
        {
            List<WorkOrder> workorder = new List<WorkOrder>();
           
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
               workorder = db.Query<WorkOrder>("SELECT * FROM WorkOrder");
            
            }
            return workorder;
        }

        
        public string SaveWorkOrder(WorkOrderViewModel workorder)
        {
            string result = string.Empty;
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                try
                {
                    var existingWO = (db.Table<WorkOrder>().Where(
                        c => c.Id.Equals(workorder.Id))).SingleOrDefault();

                    // Check if it is an existing work order, if so update 
                    if (existingWO != null)
                    {
                        existingWO.Suspend_Reason__c = workorder.reason;
                        existingWO.Work_Status__c = workorder.status;
                        existingWO.WorkOrder_End_Date__c = workorder.endDate;
                        if (Comments__c != null)
                        {
                            existingWO.Comments__c = workorder.Comments__c;
                        }
                        existingWO.CustomerAvailability__c = workorder.CustomerAvailability__c;
                        existingWO.IsProductReplaced__c = workorder.IsProductReplaced__c;
                       
                        existingWO.ProblemOptions__c = workorder.ProblemOptions__c;

                        int success = db.Update(existingWO);
                    }
                     // else insert 
                    else
                    {
                        int success = db.Insert(new WorkOrder()
                        {
                            Id=workorder.id,
                            Suspend_Reason__c = workorder.reason,
                            Work_Status__c = workorder.status,
                            WorkOrder_End_Date__c = workorder.endDate,
                            Comments__c = workorder.Comments__c,
                            CustomerAvailability__c = workorder.CustomerAvailability__c,
                            IsProductReplaced__c = workorder.IsProductReplaced__c,
                            ProblemOptions__c = workorder.ProblemOptions__c
                        });
                    }
                    result = "Success";
                }
                catch(Exception ex)
                {
                    result = "This Workorder was not saved.";
                }
            }
            return result;
        }

        public string DeleteWorkOrder(String WorkOrderId)
        {
            string result = string.Empty;
            if (null != WorkOrderId)
            {
                using (var db = new SQLite.SQLiteConnection(App.DBPath))
                {
                    var WO = db.Table<WorkOrder>().Where(
                        p => p.Id == WorkOrderId);
                   
                    db.RunInTransaction(() =>
                    {
                        foreach (WorkOrder Work in WO)
                        {
                            db.Delete(Work);
                        }


                    });
                }
            }
            return result;
        }

        // Delete all the data in SQlite
        private void DeleteAll(object sender, RoutedEventArgs e)
        {
           
            using (var db = new SQLite.SQLiteConnection(App.DBPath))
            {
                db.DeleteAll<WorkOrder>();

            }
           
        } 

    } 
}
