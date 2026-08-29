<script lang="ts">
	import { page } from '$app/state';
	import { Button } from 'flowbite-svelte';

	const errors = {
		403: {
			title: 'Acesso negado',
			message: 'Você não tem permissão para acessar esta página.'
		},
		404: {
			title: 'Página não encontrada',
			message: 'A página que você está procurando não existe.'
		},
		500: {
			title: 'Erro interno',
			message: 'Ocorreu um erro inesperado. Tente novamente mais tarde.'
		}
	} as const;

	const status = $derived(page.status);

	const error = $derived(
		errors[status as keyof typeof errors] ?? {
			title: 'Ocorreu um erro',
			message: 'Não foi possível processar sua solicitação.'
		}
	);
</script>

<svelte:head>
	<title>{status} - {error.title} | EstudaKi</title>
</svelte:head>

<div class="flex min-h-[70vh] flex-col items-center justify-center text-center">

	<span class="text-8xl font-extrabold text-primary-600">
		{status}
	</span>

	<h1 class="mt-4 text-3xl font-bold">
		{error.title}
	</h1>

	<p class="mt-3 max-w-md text-gray-500 dark:text-gray-400">
		{error.message}
	</p>

	<Button href="/" class="mt-6">
		Voltar para o início
	</Button>

</div>