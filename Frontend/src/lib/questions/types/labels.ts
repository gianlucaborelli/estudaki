import { QuestionType } from '$lib/questions/types/question-types';
import { ExamCategory } from '$lib/questions/types/exam-category';
import { QuestionIssueStatus, QuestionIssueType } from '$lib/questions/types/question-issue';

const questionTypeLabel: Record<QuestionType, string> = {
    [QuestionType.MultipleChoice]: 'Múltipla escolha',
    [QuestionType.OpenEnded]: 'Dissertativa',
    [QuestionType.Redaction]: 'Redação'
};

const examCategoryLabel: Record<ExamCategory, string> = {
    [ExamCategory.BarExam]: 'Exame da Ordem',
    [ExamCategory.PublicServiceExam]: 'Concurso público',
    [ExamCategory.NationalExam]: 'ENEM',
    [ExamCategory.SchoolExam]: 'Exames escolares',
    [ExamCategory.UniversityEntranceExam]: 'Vestibular'
};

const questionIssueTypeLabel: Record<QuestionIssueType, string> = {
    [QuestionIssueType.WrongAnswer]: 'Resposta incorreta',
    [QuestionIssueType.AmbiguousQuestion]: 'Questão ambígua',
    [QuestionIssueType.Typo]: 'Erro de digitação',
    [QuestionIssueType.Formatting]: 'Problema de formatação',
    [QuestionIssueType.MissingContent]: 'Conteúdo ausente',
    [QuestionIssueType.IncorrectStatement]: 'Enunciado incorreto',
    [QuestionIssueType.Duplicate]: 'Questão duplicada',
    [QuestionIssueType.BrokenImage]: 'Imagem quebrada',
    [QuestionIssueType.Other]: 'Outro'
};

const questionIssueStatusLabel: Record<QuestionIssueStatus, string> = {
    [QuestionIssueStatus.Pending]: 'Pendente',
    [QuestionIssueStatus.InReview]: 'Em análise',
    [QuestionIssueStatus.Resolved]: 'Resolvido',
    [QuestionIssueStatus.Rejected]: 'Rejeitado'
};

function getLabel<T extends string>(
    labels: Record<T, string>,
    value: T
): string {
    return labels[value];
}

export function getQuestionTypeLabel(type: QuestionType): string {
    return getLabel(questionTypeLabel, type);
}

export function getExamCategoryLabel(category: ExamCategory): string {
    return getLabel(examCategoryLabel, category);
}

export function getQuestionIssueTypeLabel(type: QuestionIssueType): string {
    return getLabel(questionIssueTypeLabel, type);
}

export function getQuestionIssueStatusLabel(status: QuestionIssueStatus): string {
    return getLabel(questionIssueStatusLabel, status);
}