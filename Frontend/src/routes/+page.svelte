<script>
	import Hero from '$lib/components/home/Hero.svelte';
	import Categories from '$lib/components/home/Categories.svelte';
	import How from '$lib/components/home/How.svelte';
	import Features from '$lib/components/home/Features.svelte';
	import Seo from '$lib/shared/seo/seo.svelte';

	const jsonLd = JSON.stringify({
		'@context': 'https://schema.org',
		'@graph': [
			{
				'@type': 'EducationalOrganization',
				name: 'EstudaKi',
				url: 'https://estudaki.com.br',
				logo: 'https://estudaki.com.br/favicon.ico',
				description:
					'Plataforma gratuita de estudos com questões de vestibulares, concursos públicos e OAB',
				offers: {
					'@type': 'Offer',
					price: '0',
					priceCurrency: 'BRL'
				}
			},
			{
				'@type': 'WebSite',
				name: 'EstudaKi',
				url: 'https://estudaki.com.br',
				potentialAction: {
					'@type': 'SearchAction',
					target: 'https://estudaki.com.br/questions?q={search_term_string}',
					'query-input': 'required name=search_term_string'
				}
			}			
		]
	});
</script>

<Seo
	title="EstudaKi - Questões Gratuitas para Vestibular, Concurso e OAB"
	description="EstudaKi é um ambiente de estudo gratuito para praticar questões de vestibulares, concursos públicos e da OAB. Comece a estudar agora, sem cadastro."
	keywords="questões vestibular, questões concurso, questões OAB, ENEM, Fuvest, simulado gratuito, estudar online, prova concurso"
	canonicalUrl="https://estudaki.com.br/"
	{jsonLd}
/>

<Hero />
<Categories />
<How />
<Features />
