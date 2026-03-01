using Microsoft.AspNetCore.Components;

namespace ProvaOnline.Components.Pages
{
    public partial class Home
    {
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
