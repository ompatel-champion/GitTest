using System.Collections.Generic;
using System.Web.Http;
using Helpers;
using Models;

namespace API
{
    public class DocumentController : ApiController
    {

        [AcceptVerbs("POST")]
        public bool SaveDocuments([FromBody] List<DocumentModel> documents)
        {
            return new Documents().SaveDocuments(documents);
        }


        [AcceptVerbs("GET")]
        public bool Delete(int id, int userId)
        { 
            return new Documents().DeleteDocument(id, userId);
        }


        [AcceptVerbs("GET")]
        public bool Delete(int id, int userId, int subscriberId)
        {
            return new Documents().DeleteDocument(id, userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public IEnumerable<DocumentModel> GetDocumentByType([FromBody] DocumentFilter filters)
        {
            return new Documents().GetDocumentsByDocType(filters.DocTypeId, filters.RefId, filters.SubscriberId);
        }


        [AcceptVerbs("GET")]
        public List<DocumentModel> GetCompanyDocuments([FromUri] int globalCompanyId, int subscriberId)
        {
            return new Documents().GetCompanyDocuments(globalCompanyId, subscriberId);
        }

    }
}
