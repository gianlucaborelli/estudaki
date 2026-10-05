import { redirect } from '@sveltejs/kit';
import { getFilterParameters, getQuestions } from '$lib/questions/service/question.service';
import { sanitizeQuestionContent } from '$lib/server/sanitize-question-content';
import type { QuestionFilters } from '$lib/questions/types/questionFilters';
import type { PageServerLoad } from './$types';

const DEFAULT_PAGE_INDEX = 1;
const DEFAULT_PAGE_SIZE = 10;

function parseBoolean(value: string | null): boolean | undefined {
    if (value === null || value === '') {
        return undefined;
    }

    return value === 'true';
}

export const load: PageServerLoad = async ({ url, fetch }) => {
    let pageIndex = Number(url.searchParams.get('page'));
    let pageSize = Number(url.searchParams.get('pageSize'));

    if (!Number.isInteger(pageIndex) || pageIndex < 1) {
        pageIndex = DEFAULT_PAGE_INDEX;
    }

    if (!Number.isInteger(pageSize) || pageSize < 1) {
        pageSize = DEFAULT_PAGE_SIZE;
    }

    // Se não estiverem na URL, normaliza a URL
    if (
        url.searchParams.get('page') !== pageIndex.toString() ||
        url.searchParams.get('pageSize') !== pageSize.toString()
    ) {
        const params = new URLSearchParams(url.searchParams);

        params.set('page', pageIndex.toString());
        params.set('pageSize', pageSize.toString());

        throw redirect(307, `${url.pathname}?${params.toString()}`);
    }

    const filters: QuestionFilters = {
        isPublished: parseBoolean(url.searchParams.get('isPublished')),
        wordKey: url.searchParams.get('wordKey') ?? undefined,
        typeQuestions: url.searchParams.getAll('typeQuestions'),
        examCategories: url.searchParams.getAll('examCategories'),
        year: url.searchParams.getAll('year').map(Number).filter(Number.isInteger),
        examinerOrganization: url.searchParams.getAll('examinerOrganization'),
        contractingOrganization: url.searchParams.getAll('contractingOrganization'),
        mainAreas: url.searchParams.getAll('mainAreas'),
        subAreas: url.searchParams.getAll('subAreas'),
        pageIndex,
        pageSize
    };

    // Busca no backend
    const data = await getQuestions(fetch, filters);
    const filterParameters = await getFilterParameters(fetch);
    const sanitizedItems = data.items.map((question) => ({
        ...question,
        statement: sanitizeQuestionContent(question.statement),
        questionSupports: question.questionSupports.map((support) => ({
            ...support,
            content: sanitizeQuestionContent(support.content)
        })),
        choices: question.choices.map((choice) => ({
            ...choice,
            explanation: sanitizeQuestionContent(choice.explanation)
        }))
    }));

    return {
        ...data,
        items: sanitizedItems,
        pageNumber: pageIndex,
        pageSize,
        filterParameters
    };
};