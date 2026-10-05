export enum QuestionIssueType {
    WrongAnswer = 'wrong-answer',
    AmbiguousQuestion = 'ambiguous-question',
    Typo = 'typo',
    Formatting = 'formatting',
    MissingContent = 'missing-content',
    IncorrectStatement = 'incorrect-statement',
    Duplicate = 'duplicate',
    BrokenImage = 'broken-image',
    Other = 'other'
}

export enum QuestionIssueStatus {
    Pending = 'pending',
    InReview = 'in-review',
    Resolved = 'resolved',
    Rejected = 'rejected'
}

export interface QuestionIssue {
    questionId: string;
    userName: string;
    userEmail: string;
    canBeReplied: boolean;
    type: QuestionIssueType;
    description?: string;
    status: QuestionIssueStatus;
    createdAt: string;
}

export interface CreateQuestionIssueRequest {
    userName: string;
    userEmail: string;
    canBeReplied: boolean;
    type: QuestionIssueType;
    description?: string;
}

export const QUESTION_ISSUE_DESCRIPTION_MAX_LENGTH = 500;

export interface QuestionIssueFormValues {
    userName: string;
    userEmail: string;
    canBeReplied: boolean;
    type: QuestionIssueType | '';
    description: string;
}

export interface QuestionIssueFormErrors {
    userName?: string;
    userEmail?: string;
    type?: string;
    description?: string;
    general?: string;
}
