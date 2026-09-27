<script lang="ts">
	interface Props {
		title?: string;
		description?: string;
		keywords?: string;
		canonicalUrl?: string;
		imageUrl?: string;
		ogType?: string;
		jsonLd?: string;
	}

	const {
		title = 'EstudaKi - Questões Gratuitas para Vestibular, Concurso e OAB',
		description = 'EstudaKi é um ambiente de estudo gratuito para praticar questões de vestibulares, concursos públicos e da OAB. Comece a estudar agora, sem cadastro.',
		keywords = 'questões, vestibular, concurso, OAB, ENEM, Fuvest, estudar, simulado, gratuito',
		canonicalUrl,
		imageUrl,
		ogType = 'website',
		jsonLd
	}: Props = $props();

	const url = $derived(canonicalUrl ?? 'https://www.estudaki.com.br/');
	const image = $derived(imageUrl ?? 'https://estudaki.com.br/favicon.ico');
	
	const jsonLdScript = $derived(
		jsonLd ? `<${'script'} type="application/ld+json">${jsonLd}<${'/script'}>` : undefined
	);
</script>

<svelte:head>
	<title>{title}</title>
	<meta name="description" content={description} />
	<meta name="keywords" content={keywords} />

	{#if canonicalUrl}
		<link rel="canonical" href={canonicalUrl} />
	{/if}

	<!-- Open Graph / Facebook -->
	<meta property="og:type" content={ogType} />
	<meta property="og:url" content={url} />
	<meta property="og:title" content={title} />
	<meta property="og:description" content={description} />
	{#if imageUrl}
		<meta property="og:image" content={image} />
	{/if}

	<!-- Twitter -->
	<meta name="twitter:card" content="summary_large_image" />
	<meta property="twitter:domain" content="estudaki.com.br" />
	<meta property="twitter:url" content={url} />
	<meta name="twitter:title" content={title} />
	<meta name="twitter:description" content={description} />
	{#if imageUrl}
		<meta name="twitter:image" content={image} />
	{/if}

	{#if jsonLd}
		<!-- eslint-disable-next-line svelte/no-at-html-tags -->
		{@html jsonLdScript}
	{/if}
</svelte:head>
