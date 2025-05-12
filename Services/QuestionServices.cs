using MongoDB.Bson;
using ProvaOnline.Data;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models;

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

        public async Task<FilterParameters> LoadFilterParameters(FilterParameters filterParameters)
        {
            var result = await _questionRepository.QueryDistinctPropertiesAsync(filterParameters);
            return result;
        }

        public async Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchFilter filterParameters)
        {
            return await _questionRepository.SearchQuestionsPaginatedAsync(filterParameters);
        }
    }
}
