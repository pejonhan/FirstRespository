using App.Core.Domain;
using App.Core.Domain.BaseObject;
using App.Core.Dtos;
using App.Core.Helpers;
using App.Services;
using App.WebApi.Apis;
using App.WebApi.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace App.Web.Apis
{
    public partial class DictionaryController : AppApiControllerBase
    {
        public ICommonService _commonService;
        public IDictionaryService _dictionaryService;
        public DictionaryController(ICommonService commonService,IDictionaryService dictionaryService)
        {
            _commonService = commonService;
            _dictionaryService = dictionaryService;
        }

        [Route("api/Dictionarys")]
        public GridResult GetDictionarys(int page, int rows, string sort = "CreationTime", string order = "asc", Guid? teamId = null, string filterRules = "")
        {
            int total;
            var where = "";

            var query = _commonService.GetPageRecords<Dictionary>(where, page, rows, sort, order, out total,
                filterRules);

            return new GridResult { total = total, rows = query };
        }

        [HttpPost]
        [Route("api/Dictionarys")]
        public async Task<IHttpActionResult> PostDictionaryCategorys(BulkDto<DictionaryDto> dto)
        {
            var result = await _dictionaryService.SaveDictionarys(dto);

            return Ok(result);
        }
    }
}
