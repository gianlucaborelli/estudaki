import { error } from '@sveltejs/kit';
import { getQuestionById } from '$lib/questions/service/question.service';
import { sanitizeQuestionContent } from '$lib/server/sanitize-question-content';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params, fetch }) => {
    const question = await getQuestionById(fetch, params.id);

    if (!question) {
        throw error(404, 'Questão não encontrada');
    }

    return {
        question: {
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
        }
    };
};
