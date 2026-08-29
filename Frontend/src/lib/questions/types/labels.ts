import { QuestionType } from '$lib/questions/types/question-types';
import { ExamCategory } from '$lib/questions/types/exam-category';

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