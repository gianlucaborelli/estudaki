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

        public Task<QuestionDocument> GetQuestionById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<QuestionDocument>> SearchQuestionsPaginatedAsync(SearchFilter filterParameters)
        {
            return await _questionRepository.SearchQuestionsPaginatedAsync(filterParameters);
        }
    }
}
