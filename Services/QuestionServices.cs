using ProvaOnline.Data;
using ProvaOnline.Helper;
using ProvaOnline.Helpers;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Services
{
    public class QuestionServices : IQuestionServices
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IPublicNoticeRepository _publicNoticeRepository;

        public QuestionServices(
            IQuestionRepository questionRepository,
            IPublicNoticeRepository publicNoticeRepository)
        {
            _questionRepository = questionRepository;
            _publicNoticeRepository = publicNoticeRepository;
        }

        public async Task<QuestionWithNoticeDto?> GetQuestionByIdAsync(string id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
                return null;

            var dto = new QuestionWithNoticeDto
            {
                Question = question
            };

            // Busca o PublicNotice se existir
            if (!string.IsNullOrEmpty(question.PublicNoticeId))
            {
                dto.PublicNotice = await _publicNoticeRepository.GetByIdAsync(question.PublicNoticeId);
            }

            return dto;
        }

        public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
        {
            return await _questionRepository.FindFilterParametersAsync(filterParameters);
        }

        public async Task<PageResult<QuestionWithNoticeDto>> SearchQuestionsPaginatedAsync(SearchParameters searchParameter)
        {
            var questionsPage = await _questionRepository.FindQuestionsPaginatedAsync(searchParameter);

            var publicNoticeIds = questionsPage.Items
                .Where(q => !string.IsNullOrEmpty(q.PublicNoticeId))
                .Select(q => q.PublicNoticeId!)
                .Distinct()
                .ToList();

            var publicNotices = await _publicNoticeRepository.GetByIdsAsync(publicNoticeIds);
            var publicNoticesDict = publicNotices.ToDictionary(p => p.Id!);

            var dtos = questionsPage.Items.Select(question =>
            {
                var dto = new QuestionWithNoticeDto
                {
                    Question = question
                };

                if (!string.IsNullOrEmpty(question.PublicNoticeId) &&
                    publicNoticesDict.TryGetValue(question.PublicNoticeId, out var publicNotice))
                {
                    dto.PublicNotice = publicNotice;
                }

                return dto;
            }).ToList();

            return new PageResult<QuestionWithNoticeDto>
            {
                Items = dtos,
                PageNumber = questionsPage.PageNumber,
                PageSize = questionsPage.PageSize,
                TotalItems = questionsPage.TotalItems
            };
        }
    }
}
