import { API_URL } from '$env/static/private';
import type { Question, } from '$lib/questions/types/question';
import type { QuestionFilters } from '$lib/questions/types/questionFilters';
import type { PaginatedResponse } from '$lib/shared/types/PaginatedResponse';

function buildQuestionsQuery(filters: QuestionFilters): string {
    const params = new URLSearchParams();

    if (filters.isPublished !== undefined) {
        params.set('isPublished', String(filters.isPublished));
    }

    if (filters.wordKey) {
        params.set('wordKey', filters.wordKey);
    }

    if (filters.pageIndex !== undefined) {
        params.set('pageIndex', String(filters.pageIndex));
    }

    if (filters.pageSize !== undefined) {
        params.set('pageSize', String(filters.pageSize));
    }

    for (const value of filters.typeQuestions ?? []) {
        params.append('typeQuestions', value);
    }

    for (const value of filters.examCategories ?? []) {
        params.append('examCategories', value);
    }

    for (const value of filters.mainAreas ?? []) {
        params.append('mainAreas', value);
    }

    for (const value of filters.subAreas ?? []) {
        params.append('subAreas', value);
    }

    return params.toString();
}

export async function getQuestions(
    fetch: typeof globalThis.fetch,
    filters: QuestionFilters = {}
): Promise<PaginatedResponse<Question>> {
    try {
        const query = buildQuestionsQuery(filters);
        const response = await fetch(`${API_URL}/api/questions?${query}`);

        if (!response.ok) {
            throw new Error(`Erro ao buscar questões: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Erro ao buscar questões:', error);
        throw error;
    }
}
