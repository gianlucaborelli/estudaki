<script lang="ts">
	import type { Snippet } from 'svelte';
	import { CloseButton } from 'flowbite-svelte';

	type Props = {
		open?: boolean;
		title?: string;
		onClose?: () => void;
		children: Snippet;
		footer?: Snippet;
	};

	let { open = $bindable(false), title, onClose, children, footer }: Props = $props();

	function close() {
		open = false;
		onClose?.();
	}

	function handleBackdropClick(event: MouseEvent) {
		if (event.target === event.currentTarget) {
			close();
		}
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Escape') {
			close();
		}
	}

	$effect(() => {
		document.body.style.overflow = open ? 'hidden' : '';

		return () => {
			document.body.style.overflow = '';
		};
	});
</script>

<svelte:window onkeydown={open ? handleKeydown : undefined} />

{#if open}
	<div class="modal-backdrop" role="presentation" onclick={handleBackdropClick}>
		<div
			class="modal-card"
			role="dialog"
			aria-modal="true"
			aria-labelledby={title ? 'modal-title' : undefined}
		>
			<div class="modal-header">
				{#if title}
					<h2 id="modal-title">{title}</h2>
				{/if}
				<CloseButton color="none" class="modal-close" onclick={close} aria-label="Fechar" />
			</div>

			<div class="modal-body">
				{@render children()}
			</div>

			{#if footer}
				<div class="modal-footer">
					{@render footer()}
				</div>
			{/if}
		</div>
	</div>
{/if}

<style>
	.modal-backdrop {
		position: fixed;
		inset: 0;
		z-index: 100;
		display: flex;
		align-items: center;
		justify-content: center;
		padding: 24px;
		background: color-mix(in srgb, black 50%, transparent);
	}

	.modal-card {
		width: 100%;
		max-width: 560px;
		max-height: calc(100vh - 48px);
		overflow-y: auto;
		border: 1px solid var(--border);
		border-radius: 20px;
		background: var(--surface-elevated);
		box-shadow: 0 25px 60px color-mix(in srgb, black 25%, transparent);
	}

	.modal-header {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 16px;
		padding: 28px 28px 0;
	}

	.modal-header h2 {
		margin: 0;
		color: var(--text);
		font-size: 1.4rem;
		font-weight: 800;
		letter-spacing: -0.02em;
	}

	:global(.modal-close) {
		flex-shrink: 0;
		border-radius: 0.5rem;
		padding: 0.375rem;
		color: var(--text-muted);
		transition:
			background-color 0.2s ease,
			color 0.2s ease;
	}

	:global(.modal-close:hover) {
		background-color: color-mix(in srgb, var(--text) 8%, transparent);
		color: var(--text);
	}

	:global(.modal-close:focus-visible) {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	.modal-body {
		padding: 20px 28px 28px;
	}

	.modal-footer {
		display: flex;
		justify-content: flex-end;
		gap: 12px;
		padding: 0 28px 28px;
	}

	@media (max-width: 640px) {
		.modal-header,
		.modal-body,
		.modal-footer {
			padding-left: 20px;
			padding-right: 20px;
		}
	}
</style>
