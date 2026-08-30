import { QuestionType } from '$lib/questions/types/question-types';
import type { ExamCategory } from '$lib/questions/types/exam-category';

export interface Question {
    questionId: string;
    publicNoticeId: string;
    examId: string;
    publicNoticeNumber: string;
    year: number;
    examinerOrganization: string;
    contractingOrganization: string;
    examCategory: ExamCategory;
    phase: string;
    positions: string[];
    area: string;
    educationLevel: string;
    publicNoticeFileUrl: string;
    examBookletUrl: string;
    answerKeyUrl: string;
    isNullified: boolean;
    questionNumber: number;
    questionType: QuestionType;
    mainArea: string;
    subAreas: string[];
    questionContents: BlockContent[];
    questionSupports: QuestionSupport[];
    choices: QuestionChoice[];
    createdAt: string;
}

export interface QuestionChoice {
    option: string;
    contentBlocks: BlockContent[];
    isCorrect: boolean;
}

export interface QuestionSupport {
    id: string;
    contents: BlockContent[];
}

export type BlockContent = ParagraphBlock | ImageBlock;

export interface ParagraphBlock {
    order: number;
    type: 'paragraph';
    text: string;
    title: string,
    source: string,
}

export interface ImageBlock {
    order: number;
    type: 'image';
    key: string;
    title: string;
    source: string;
    description: string;
}