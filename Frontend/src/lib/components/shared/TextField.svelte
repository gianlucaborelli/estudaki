<script lang="ts">
	import type { Snippet } from 'svelte';

	type TextFieldType = 'text' | 'email' | 'tel' | 'password' | 'number' | 'search';

	type Props = {
		label?: string;
		type?: TextFieldType;
		name?: string;
		id?: string;
		placeholder?: string;
		value?: string;
		error?: string;
		helperText?: string;
		required?: boolean;
		multiline?: boolean;
		rows?: number;
		maxlength?: number;
		class?: string;
		icon?: Snippet;
		trailing?: Snippet;
	};

	let {
		label,
		type = 'text',
		name,
		id,
		placeholder,
		value = $bindable(''),
		error,
		helperText,
		required = false,
		multiline = false,
		rows = 4,
		maxlength,
		class: className = '',
		icon,
		trailing
	}: Props = $props();

	const autoId = $props.id();
	const fieldId = $derived(id ?? autoId);
	const errorId = $derived(`${fieldId}-error`);
</script>

<div class={`text-field ${className}`}>
	{#if label}
		<label for={fieldId} class="text-field-label">{label}</label>
	{/if}

	<div class="text-field-box" class:has-error={!!error} class:has-trailing={!!trailing}>
		{#if icon}
			<span class="text-field-icon">{@render icon()}</span>
		{/if}

		{#if multiline}
			<textarea
				id={fieldId}
				{name}
				{placeholder}
				{required}
				{rows}
				{maxlength}
				bind:value
				aria-invalid={!!error}
				aria-describedby={error ? errorId : undefined}
				class="text-field-input"></textarea>
		{:else}
			<input
				id={fieldId}
				{name}
				{type}
				{placeholder}
				{required}
				{maxlength}
				bind:value
				aria-invalid={!!error}
				aria-describedby={error ? errorId : undefined}
				class="text-field-input"
			/>
		{/if}

		{#if trailing}
			<div class="text-field-trailing">{@render trailing()}</div>
		{/if}
	</div>

	{#if error}
		<span id={errorId} class="text-field-message error">{error}</span>
	{:else if helperText}
		<span class="text-field-message">{helperText}</span>
	{/if}
</div>

<style>
	.text-field {
		display: grid;
		gap: 0.5rem;
	}

	.text-field-label {
		color: var(--text);
		font-size: 0.875rem;
		font-weight: 600;
		line-height: 1.25;
	}

	.text-field-box {
		display: flex;
		align-items: center;
		gap: 10px;

		border: 1px solid var(--border);
		border-radius: 16px;

		background: var(--surface-elevated);

		transition:
			border-color 0.2s ease,
			box-shadow 0.2s ease;
	}

	.text-field-box:focus-within {
		border-color: var(--primary);
		box-shadow: 0 0 0 4px color-mix(in srgb, var(--primary) 12%, transparent);
	}

	.text-field-box.has-error {
		border-color: var(--secondary);
	}

	.text-field-box.has-error:focus-within {
		box-shadow: 0 0 0 4px color-mix(in srgb, var(--secondary) 14%, transparent);
	}

	.text-field-icon {
		display: flex;
		flex-shrink: 0;
		align-items: center;

		padding-left: 10px;

		color: var(--text-muted);

		font-size: 1.5rem;
		line-height: 1;
	}

	.text-field-input {
		flex: 1;
		min-width: 0;

		border: none;
		outline: none;

		background: transparent;

		padding: 8px 14px;

		color: var(--text);

		font-family: inherit;
		font-size: 0.9rem;
		resize: vertical;
	}

	.text-field-box:not(.has-trailing) .text-field-input {
		padding-right: 16px;
	}

	.text-field-input::placeholder {
		color: var(--text-muted);
		opacity: 0.8;
	}

	.text-field-trailing {
		flex-shrink: 0;
	}

	.text-field-message {
		color: var(--text-muted);
		font-size: 0.8rem;
	}

	.text-field-message.error {
		color: var(--secondary);
	}
</style>
