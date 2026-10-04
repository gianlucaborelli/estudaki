<script lang="ts">
	import { enhance } from '$app/forms';
	import { Button, Toggle } from 'flowbite-svelte';
	import Modal from '$lib/components/shared/Modal.svelte';
	import TextField from '$lib/components/shared/TextField.svelte';
	import CustomSelect from '$lib/components/shared/CustomSelect.svelte';
	import { getQuestionIssueTypeLabel } from '$lib/questions/types/labels';
	import {
		QUESTION_ISSUE_DESCRIPTION_MAX_LENGTH,
		QuestionIssueType,
		type QuestionIssueFormErrors,
		type QuestionIssueFormValues
	} from '$lib/questions/types/question-issue';

	type Props = {
		questionId: string;
		open?: boolean;
	};

	let { questionId, open = $bindable(false) }: Props = $props();

	const issueTypeOptions = Object.values(QuestionIssueType).map((value) => ({
		value,
		name: getQuestionIssueTypeLabel(value)
	}));

	type IssueFormValues = Omit<QuestionIssueFormValues, 'type'>;

	function emptyValues(): IssueFormValues {
		return { userName: '', userEmail: '', description: '', canBeReplied: false };
	}

	let values = $state(emptyValues());
	let selectedIssueType = $state<QuestionIssueType[]>([]);
	let errors = $state<QuestionIssueFormErrors>({});
	let submitting = $state(false);
	let success = $state(false);

	$effect(() => {
		if (!open) {
			values = emptyValues();
			selectedIssueType = [];
			errors = {};
			success = false;
		}
	});
</script>

<Modal bind:open title="Sinalizar questão" onClose={() => (open = false)}>
	{#if success}
		<p class="issue-success">
			Sinalização enviada com sucesso! Obrigado por ajudar a melhorar o EstudaKi.
		</p>
	{:else}
		{#if errors.general}
			<p class="issue-error">{errors.general}</p>
		{/if}

		<form
			method="POST"
			action="/questions/{questionId}?/signalizeQuestionIssue"
			use:enhance={() => {
				submitting = true;
				errors = {};

				return async ({ result }) => {
					submitting = false;

					if (result.type === 'success') {
						success = true;
						return;
					}

					if (result.type === 'failure') {
						errors = (result.data?.errors as QuestionIssueFormErrors) ?? {};
						return;
					}

					errors = { general: 'Não foi possível enviar sua sinalização. Tente novamente.' };
				};
			}}
		>
			<TextField
				label="Nome"
				name="userName"
				bind:value={values.userName}
				required
				error={errors.userName}
			/>

			<TextField
				label="Email"
				name="userEmail"
				type="email"
				bind:value={values.userEmail}
				required
				error={errors.userEmail}
			/>

			<CustomSelect
				label="Tipo de problema"
				items={issueTypeOptions}
				bind:value={selectedIssueType}
				multiple={false}
				name="type"
				required
				placeholder="Selecione o tipo de problema"
			/>
			{#if errors.type}
				<span class="issue-field-error">{errors.type}</span>
			{/if}

			<TextField
				label="Descrição (opcional)"
				name="description"
				multiline
				rows={4}
				bind:value={values.description}
				maxlength={QUESTION_ISSUE_DESCRIPTION_MAX_LENGTH}
				error={errors.description}
			/>

			<Toggle name="canBeReplied" value="true" bind:checked={values.canBeReplied}>
				Aceito receber uma resposta sobre essa sinalização
			</Toggle>

			<div class="issue-actions">
				<Button
					type="button"
					class="issue-cancel"
					disabled={submitting}
					onclick={() => (open = false)}
				>
					Cancelar
				</Button>
				<Button type="submit" class="issue-submit" disabled={submitting} loading={submitting}>
					Enviar sinalização
				</Button>
			</div>
		</form>
	{/if}
</Modal>

<style>
	form {
		display: flex;
		flex-direction: column;
		gap: 20px;
	}

	.issue-success {
		margin: 0;
		color: var(--text);
		line-height: 1.6;
	}

	.issue-error {
		margin: 0;
		color: var(--secondary);
		font-size: 0.875rem;
	}

	.issue-field-error {
		margin-top: -12px;
		color: var(--secondary);
		font-size: 0.8rem;
	}

	.issue-actions {
		display: flex;
		justify-content: flex-end;
		gap: 12px;
	}

	:global(.issue-submit) {
		padding: 12px 24px;
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

	:global(.issue-submit:hover:not(:disabled)) {
		background: var(--secondary-hover);
		transform: translateY(-1px);
	}

	:global(.issue-submit:disabled) {
		opacity: 0.7;
		cursor: not-allowed;
		transform: none;
	}

	:global(.issue-cancel) {
		padding: 12px 24px;
		border: 1px solid var(--border);
		border-radius: 11px;
		background: transparent;
		color: var(--text);
		font-family: inherit;
		font-size: 0.9rem;
		font-weight: 700;
		transition:
			background-color 0.2s ease,
			border-color 0.2s ease;
	}

	:global(.issue-cancel:hover:not(:disabled)) {
		background-color: color-mix(in srgb, var(--text) 6%, transparent);
		border-color: var(--secondary);
	}

	:global(.issue-cancel:disabled) {
		opacity: 0.6;
		cursor: not-allowed;
	}
</style>
