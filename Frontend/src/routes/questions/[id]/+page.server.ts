import { error, fail } from '@sveltejs/kit';
import { createQuestionIssue, getQuestionById } from '$lib/questions/service/question.service';
import { sanitizeQuestionContent } from '$lib/server/sanitize-question-content';
import { QuestionIssueType, QUESTION_ISSUE_DESCRIPTION_MAX_LENGTH } from '$lib/questions/types/question-issue';
import type { QuestionIssueFormErrors, QuestionIssueFormValues } from '$lib/questions/types/question-issue';
import type { Actions, PageServerLoad } from './$types';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function validateIssue(values: QuestionIssueFormValues): QuestionIssueFormErrors {
    const errors: QuestionIssueFormErrors = {};

    if (!values.userName) {
        errors.userName = 'Informe seu nome.';
    }

    if (!values.userEmail) {
        errors.userEmail = 'Informe seu e-mail.';
    } else if (!EMAIL_REGEX.test(values.userEmail)) {
        errors.userEmail = 'Informe um e-mail válido.';
    }

    if (!values.type || !Object.values(QuestionIssueType).includes(values.type as QuestionIssueType)) {
        errors.type = 'Selecione o tipo do problema.';
    }

    if (values.description.length > QUESTION_ISSUE_DESCRIPTION_MAX_LENGTH) {
        errors.description = `A descrição deve ter no máximo ${QUESTION_ISSUE_DESCRIPTION_MAX_LENGTH} caracteres.`;
    }

    return errors;
}

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

export const actions: Actions = {
    signalizeQuestionIssue: async ({ request, params, fetch }) => {
        const formData = await request.formData();

        const values: QuestionIssueFormValues = {
            userName: String(formData.get('userName') ?? '').trim(),
            userEmail: String(formData.get('userEmail') ?? '').trim(),
            type: String(formData.get('type') ?? '') as QuestionIssueType | '',
            description: String(formData.get('description') ?? '').trim(),
            canBeReplied: formData.get('canBeReplied') === 'true'
        };

        const errors = validateIssue(values);

        if (Object.keys(errors).length > 0) {
            return fail(400, { errors, values });
        }

        try {
            await createQuestionIssue(fetch, params.id, {
                userName: values.userName,
                userEmail: values.userEmail,
                canBeReplied: values.canBeReplied,
                type: values.type as QuestionIssueType,
                description: values.description || undefined
            });
        } catch (err) {
            console.error('Erro ao sinalizar questão:', err);

            const serverError: QuestionIssueFormErrors = {
                general: 'Não foi possível enviar sua sinalização. Tente novamente.'
            };

            return fail(500, { errors: serverError, values });
        }

        return { success: true };
    }
};

