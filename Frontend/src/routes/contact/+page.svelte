<script lang="ts">
	import { enhance } from '$app/forms';
	import { Alert, Button, Toggle } from 'flowbite-svelte';
	import Seo from '$lib/shared/seo/seo.svelte';
	import TextField from '$lib/components/shared/TextField.svelte';
	import { CONTACT_MESSAGE_MAX_LENGTH } from '$lib/contact/types/contact';
	import type { ActionData } from './$types';

	let { form }: { form: ActionData } = $props();

	let name = $state('');
	let email = $state('');
	let message = $state('');
	let canBeReplied = $state(false);
	let submitting = $state(false);

	$effect(() => {
		name = form?.values?.name ?? '';
		email = form?.values?.email ?? '';
		message = form?.values?.message ?? '';
		canBeReplied = form?.values?.canBeReplied ?? false;
	});

	const remainingCharacters = $derived(CONTACT_MESSAGE_MAX_LENGTH - message.length);
</script>

<Seo
	title="EstudaKi - Contato"
	description="Entre em contato com a equipe do EstudaKi para dúvidas, sugestões ou parcerias."
/>

<section class="section contact-section">
	<div class="section-header">
		<span class="eyebrow">FALE CONOSCO</span>

		<h1>
			Vamos conversar
			<span>sobre o EstudaKi?</span>
		</h1>

		<p>
			Dúvidas, sugestões ou parcerias — preencha o formulário abaixo e retornaremos o mais breve
			possível.
		</p>
	</div>

	<div class="contact-card">
		{#if form?.success || form?.errors?.general}
			<div class="form-alerts">
				{#if form?.success}
					<Alert color="green" dismissable>Mensagem enviada com sucesso!</Alert>
				{/if}

				{#if form?.errors?.general}
					<Alert color="red" dismissable>{form.errors.general}</Alert>
				{/if}
			</div>
		{/if}

		<form
			method="POST"
			use:enhance={() => {
				submitting = true;

				return async ({ update }) => {
					await update();
					submitting = false;
				};
			}}
		>
			<TextField
				label="Nome"
				name="name"
				type="text"
				bind:value={name}
				required
				error={form?.errors?.name}
			/>

			<TextField
				label="Email"
				name="email"
				type="email"
				bind:value={email}
				required
				error={form?.errors?.email}
			/>

			<TextField
				label="Mensagem"
				name="message"
				multiline
				rows={8}
				bind:value={message}
				maxlength={CONTACT_MESSAGE_MAX_LENGTH}
				required
				error={form?.errors?.message}
				helperText={`${remainingCharacters} caracteres restantes`}
			/>

			<Toggle name="canBeReplied" value="true" bind:checked={canBeReplied}>
				Aceito ser respondido por e-mail
			</Toggle>

			<Button type="submit" class="contact-submit" disabled={submitting} loading={submitting}>
				Enviar mensagem
			</Button>
		</form>
	</div>
</section>

<style>
	:global(body) {
		overflow-x: hidden;
	}

	.section {
		max-width: 1440px;
		margin: 0 auto;
		padding: 90px 24px;
	}

	.section-header {
		max-width: 600px;
		margin: 0 auto 45px;
		text-align: center;
	}

	.eyebrow {
		display: inline-block;
		margin-bottom: 14px;
		color: var(--secondary);
		font-size: 0.8rem;
		font-weight: 700;
		letter-spacing: 0.08em;
	}

	.section-header h1 {
		margin: 0;
		color: var(--text);
		font-size: clamp(2.2rem, 4vw, 3.4rem);
		line-height: 1.05;
		letter-spacing: -0.04em;
		font-weight: 800;
	}

	.section-header h1 span {
		display: block;
		color: var(--primary);
	}

	.section-header p {
		margin-top: 18px;
		color: var(--text-muted);
		line-height: 1.7;
	}

	.contact-card {
		max-width: 620px;
		margin: 0 auto;
		padding: 40px;
		border: 1px solid var(--border);
		border-radius: 20px;
		background: var(--surface-elevated);
		box-shadow: 0 15px 40px color-mix(in srgb, var(--text) 7%, transparent);
	}

	.form-alerts {
		display: flex;
		flex-direction: column;
		gap: 12px;
		margin-bottom: 22px;
	}

	form {
		display: flex;
		flex-direction: column;
		gap: 22px;
	}

	:global(.contact-submit) {
		align-self: flex-start;
		padding: 13px 28px;
		border: none;
		border-radius: 11px;
		background: var(--secondary);
		color: white;
		font-family: inherit;
		font-size: 0.9rem;
		font-weight: 700;
		transition:
			background 0.2s ease,
			transform 0.2s ease;
	}

	:global(.contact-submit:hover:not(:disabled)) {
		background: var(--secondary-hover);
		transform: translateY(-1px);
	}

	:global(.contact-submit:disabled) {
		opacity: 0.7;
		cursor: not-allowed;
		transform: none;
	}

	@media (max-width: 640px) {
		.contact-card {
			padding: 28px 20px;
		}
	}
</style>
