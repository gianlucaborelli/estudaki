import { API_URL } from '$env/static/private';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = async ({ fetch }) => {
    const response = await fetch(`${API_URL}/sitemap.xml`);

    if (!response.ok) {
        return new Response('Sitemap indisponível', { status: response.status });
    }

    return new Response(response.body, {
        status: response.status,
        headers: {
            'Content-Type': 'application/xml'
        }
    });
};
