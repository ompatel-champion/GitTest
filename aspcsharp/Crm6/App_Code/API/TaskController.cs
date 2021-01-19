using Crm6.App_Code.Shared;
using Helpers;
using Models;
using System.Collections.Generic; 
using System.Web.Http;

namespace API
{
    public class TaskController : ApiController
    {


        [AcceptVerbs("POST")]
        public int SaveTask([FromBody]ActivityModel taskitem)
        {
            return new Tasks().SaveTask(taskitem);
        }


        [AcceptVerbs("POST")]
        public List< Activity> GetTasks(TaskFilter filters)
        {
            return new Tasks().GetTasks(filters);
        }

        [AcceptVerbs("GET")]
        public ActivityModel GetTask([FromUri]int taskId, int subscriberId)
        {
            return new Tasks().GetTask(taskId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteTask([FromUri]int taskId, int userId, int subscriberId)
        {
            return new Tasks().DeleteTask(taskId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool CompleteTask([FromUri]int taskId, int userId, int subscriberId, bool revert=false)
        {
            return new Tasks().CompleteTask(taskId, subscriberId, userId, revert);
        }

        [AcceptVerbs("GET")]
        public bool ToggleTaskCompleted([FromUri]int taskId, bool state, int userId, int subscriberId)
        {
            return new Tasks().ToggleTaskCompleted(taskId, state, subscriberId, userId);
        }
    }
}
