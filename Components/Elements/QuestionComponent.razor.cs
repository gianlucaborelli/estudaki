using Microsoft.AspNetCore.Components;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Components.Elements
{

    public partial class QuestionComponent : ComponentBase
    {
        [Parameter]
        public QuestionWithNoticeDto? Value { get; set; }

        protected bool _showAnswers = false;

    }
}
