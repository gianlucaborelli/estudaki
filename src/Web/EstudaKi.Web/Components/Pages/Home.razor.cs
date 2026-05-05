using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using EstudaKi.Web.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Domain.Common;

namespace EstudaKi.Web.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IQueryDispatcher QueryDispatcher { get; set; } = default!;

        private string SearchQuery { get; set; } = string.Empty;

        private IReadOnlyCollection<string> SelectedTypeQuestion { get; set; } = [];
        private IReadOnlyCollection<string> SelectedExamCategory { get; set; } = [];
        private IEnumerable<string> SelectedMainArea { get; set; } = [];
        private IEnumerable<string> SelectedSubArea { get; set; } = [];

        private bool _showFilters { get; set; } = false;

        private List<(string Value, string DisplayName)> AvailableTypeQuestionsDisplay { get; set; } = [];
        private string[] AvailableTypeQuestions { get; set; } = [];
        private List<(string Value, string DisplayName)> AvailableExamCategoriesDisplay { get; set; } = [];
        private string[] AvailableExamCategories { get; set; } = [];
        private string[] AvailableMainAreas { get; set; } = [];
        private string[] AvailableSubAreas { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            await LoadAvailableFilters();
        }
        protected async Task LoadingFilterParameters()
        {
            await LoadAvailableFilters();
        }

        private async Task LoadAvailableFilters()
        {
            try
            {
                var filterParams = new FilterParameters
                {
                    TypeQuestions = SelectedTypeQuestion?.ToArray() ?? Array.Empty<string>(),
                    ExamCategories = SelectedExamCategory?.ToArray() ?? Array.Empty<string>(),
                    MainAreas = SelectedMainArea?.ToArray() ?? Array.Empty<string>(),
                    SubAreas = SelectedSubArea?.ToArray() ?? Array.Empty<string>()
                };

                var result = await QueryDispatcher
                    .DispatchAsync<GetFilterParametersQuery, FilterParameters>(
                        new GetFilterParametersQuery(filterParams));

                AvailableTypeQuestions = result.TypeQuestions.ToArray();
                AvailableTypeQuestionsDisplay = QuestionTypeHelper.GetDisplayList((string[])AvailableTypeQuestions);
                AvailableExamCategories = result.ExamCategories.ToArray();    
                AvailableExamCategoriesDisplay = ExamCategoryHelper.GetDisplayList((string[])AvailableExamCategories);
                AvailableMainAreas = result.MainAreas;
                AvailableSubAreas = result.SubAreas;
            }
            catch
            {
                AvailableTypeQuestions = [];
                AvailableExamCategories = [];
                AvailableMainAreas = [];
                AvailableSubAreas = [];
            }
        }        

        private void OnSearchKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                ExecuteSearch();
            }
        }

        private void ExecuteSearch()
        {
            var queryParams = new Dictionary<string, string?>();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                queryParams["q"] = SearchQuery;
            }

            if (SelectedTypeQuestion != null && SelectedTypeQuestion.Any())
            {
                queryParams["types"] = string.Join(",", SelectedTypeQuestion);
            }

            if (SelectedExamCategory != null && SelectedExamCategory.Any())
            {
                queryParams["categories"] = string.Join(",", SelectedExamCategory);
            }

            if (SelectedMainArea != null && SelectedMainArea.Any())
            {
                queryParams["areas"] = string.Join(",", SelectedMainArea);
            }

            if (SelectedSubArea != null && SelectedSubArea.Any())
            {
                queryParams["subareas"] = string.Join(",", SelectedSubArea);
            }

            var url = QueryHelpers.AddQueryString("/result", queryParams);
            Navigation.NavigateTo(url);
        }

        private async Task ClearFilters()
        {
            SearchQuery = string.Empty;
            SelectedTypeQuestion = [];
            SelectedExamCategory = [];
            SelectedMainArea = [];
            SelectedSubArea = [];
            await LoadAvailableFilters();
        }

        private string GetStructuredData()
        {
            return @"{
                ""@context"": ""https://schema.org"",
                ""@graph"": [
                    {
                        ""@type"": ""EducationalOrganization"",
                        ""name"": ""EstudaKi"",
                        ""url"": ""https://estudaki.com.br"",
                        ""logo"": ""https://estudaki.com.br/favicon.ico"",
                        ""description"": ""Plataforma gratuita de estudos com questões de vestibulares, concursos públicos e OAB"",
                        ""offers"": {
                            ""@type"": ""Offer"",
                            ""price"": ""0"",
                            ""priceCurrency"": ""BRL""
                        }
                    },
                    {
                        ""@type"": ""WebSite"",
                        ""name"": ""EstudaKi"",
                        ""url"": ""https://estudaki.com.br"",
                        ""potentialAction"": {
                            ""@type"": ""SearchAction"",
                            ""target"": ""https://estudaki.com.br/result?q={search_term_string}"",
                            ""query-input"": ""required name=search_term_string""
                        }
                    },
                    {
                        ""@type"": ""FAQPage"",
                        ""mainEntity"": [
                            {
                                ""@type"": ""Question"",
                                ""name"": ""Preciso criar uma conta para usar?"",
                                ""acceptedAnswer"": {
                                    ""@type"": ""Answer"",
                                    ""text"": ""Não! Todo o conteúdo está disponível gratuitamente e sem necessidade de login.""
                                }
                            },
                            {
                                ""@type"": ""Question"",
                                ""name"": ""Quais tipos de prova estão disponíveis?"",
                                ""acceptedAnswer"": {
                                    ""@type"": ""Answer"",
                                    ""text"": ""Você encontrará questões de vestibulares (como ENEM e Fuvest), concursos públicos e da 1ª fase da OAB.""
                                }
                            },
                            {
                                ""@type"": ""Question"",
                                ""name"": ""Já posso montar simulados personalizados?"",
                                ""acceptedAnswer"": {
                                    ""@type"": ""Answer"",
                                    ""text"": ""Ainda não, mas essa funcionalidade será lançada em breve.""
                                }
                            },
                            {
                                ""@type"": ""Question"",
                                ""name"": ""Posso comentar as questões?"",
                                ""acceptedAnswer"": {
                                    ""@type"": ""Answer"",
                                    ""text"": ""Ainda não é possível, mas essa é outra funcionalidade planejada para as próximas atualizações.""
                                }
                            }
                        ]
                    }
                ]
            }";
        }
    }
}
