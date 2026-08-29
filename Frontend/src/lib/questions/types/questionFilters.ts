export interface QuestionFilters {
    isPublished?: boolean;
    wordKey?: string;
    typeQuestions?: string[];
    examCategories?: string[];
    mainAreas?: string[];
    subAreas?: string[];
    pageIndex?: number;
    pageSize?: number;
}