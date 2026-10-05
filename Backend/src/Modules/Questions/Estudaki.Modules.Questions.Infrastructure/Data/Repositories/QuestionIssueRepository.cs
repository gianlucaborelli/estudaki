using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class QuestionIssueRepository : MongoRepositoryBase<QuestionIssue>, IQuestionIssueRepository
{
    public QuestionIssueRepository(IMongoContext context) : base(context)
    {
    }
}
