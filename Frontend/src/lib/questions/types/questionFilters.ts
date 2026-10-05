export interface QuestionFilters {
    isPublished?: boolean;
    wordKey?: string;
    year?: number[];
    contractingOrganization?: string[];
    examinerOrganization?: string[];
    typeQuestions?: string[];
    examCategories?: string[];
    mainAreas?: string[];
    subAreas?: string[];
    pageIndex?: number;
    pageSize?: number;
}