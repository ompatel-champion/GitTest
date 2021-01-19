using Helpers;
using Models;
using System.Web.Http;

namespace API
{
    public class NoteController : ApiController
    {

        [AcceptVerbs("POST")]
        public NoteListResponse GetNotes([FromBody]NoteFilter filters)
        {
            return new Notes().GetNotes(filters);
        }


      

        [AcceptVerbs("POST")]
        public int SaveNote([FromBody]Crm6.App_Code.Shared.Activity note)
        {
            return new Notes().SaveNote(note);
        }


        [AcceptVerbs("GET")]
        public bool DeleteNote([FromUri]int noteId, int globalUserId)
        {
            return new Notes().DeleteNote(noteId, globalUserId);
        }

    }
}
