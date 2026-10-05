import { redirect } from '@sveltejs/kit';
import type { PageServerLoad } from './$types';

// URL antiga (/question/{id}) mantida como redirect permanente para preservar SEO
export const load: PageServerLoad = async ({ params }) => {
    throw redirect(301, `/questions/${params.id}`);
};
