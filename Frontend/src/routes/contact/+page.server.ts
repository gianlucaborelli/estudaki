import { fail } from '@sveltejs/kit';
import { createContact } from '$lib/contact/service/contact.service';
import { CONTACT_MESSAGE_MAX_LENGTH } from '$lib/contact/types/contact';
import type { ContactFormErrors, ContactFormValues } from '$lib/contact/types/contact';
import type { Actions } from './$types';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function validate(values: ContactFormValues): ContactFormErrors {
    const errors: ContactFormErrors = {};

    if (!values.name) {
        errors.name = 'Informe seu nome.';
    }

    if (!values.email) {
        errors.email = 'Informe seu e-mail.';
    } else if (!EMAIL_REGEX.test(values.email)) {
        errors.email = 'Informe um e-mail válido.';
    }

    if (!values.message) {
        errors.message = 'Informe sua mensagem.';
    } else if (values.message.length > CONTACT_MESSAGE_MAX_LENGTH) {
        errors.message = `A mensagem deve ter no máximo ${CONTACT_MESSAGE_MAX_LENGTH} caracteres.`;
    }

    return errors;
}

export const actions: Actions = {
    default: async ({ request, fetch }) => {
        const formData = await request.formData();

        const values: ContactFormValues = {
            name: String(formData.get('name') ?? '').trim(),
            email: String(formData.get('email') ?? '').trim(),
            message: String(formData.get('message') ?? '').trim(),
            canBeReplied: formData.get('canBeReplied') === 'true'
        };

        const errors = validate(values);

        if (Object.keys(errors).length > 0) {
            return fail(400, { errors, values });
        }

        try {
            await createContact(fetch, values);
        } catch (error) {
            console.error('Erro ao enviar contato:', error);

            const serverError: ContactFormErrors = {
                general: 'Não foi possível enviar sua mensagem. Tente novamente.'
            };

            return fail(500, { errors: serverError, values });
        }

        return { success: true };
    }
};
