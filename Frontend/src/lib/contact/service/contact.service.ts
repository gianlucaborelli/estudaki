import { API_URL } from '$env/static/private';
import type { Contact } from '$lib/contact/types/contact';

export async function createContact(fetch: typeof globalThis.fetch, contact: Contact): Promise<void> {
    const response = await fetch(`${API_URL}/api/contacts`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(contact)
    });

    if (!response.ok) {
        throw new Error(`Erro ao enviar contato: ${response.status}`);
    }
}
