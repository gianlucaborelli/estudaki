using MongoDB.Bson;
using ProvaOnline.Data;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Services
{
    public class QuestionServices : IQuestionServices
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionServices(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public Task<QuestionDocument?> GetQuestionById(string id)
        {
            var objectId = ObjectId.TryParse(id, out var parsedId) ? parsedId : ObjectId.Empty;
            return _questionRepository.GetByIdAsync(objectId);
        }

        public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
        {            
            return await _questionRepository.FindFilterParametersAsync(filterParameters);
        }

        public async Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchParameters searchParameter)
        {
            return await _questionRepository.FindQuestionsPaginatedAsync(searchParameter);
        }
    }
}
