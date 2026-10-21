<script lang="ts">
	import { FileSearchOutline } from 'flowbite-svelte-icons';

	type Props = {
		onClearFilters: () => void;
		onBackToFilters: () => void;
	};

	let { onClearFilters, onBackToFilters }: Props = $props();
</script>

<div class="empty-state" role="status" aria-live="polite">
	<FileSearchOutline class="empty-state-icon" />
	<h2>Nenhuma questão encontrada</h2>
	<p>
		Não encontramos questões com os filtros selecionados. Tente alterar os critérios ou limpar os
		filtros para ampliar sua pesquisa.
	</p>
	<div class="empty-state-actions">
		<button type="button" class="empty-state-primary" onclick={onClearFilters}>
			Limpar filtros
		</button>
		<button type="button" class="empty-state-secondary" onclick={onBackToFilters}>
			Voltar aos filtros
		</button>
	</div>
</div>

<style>
	.empty-state {
		display: grid;
		justify-items: center;
		gap: 0.5rem;
		max-width: 28rem;
		margin: 2.5rem auto;
		padding: 2rem 1.5rem;
		text-align: center;
	}

	:global(.empty-state-icon) {
		width: 2.75rem;
		height: 2.75rem;
		margin-bottom: 0.5rem;
		color: var(--tertiary);
	}

	.empty-state h2 {
		margin: 0;
		color: var(--text);
		font-size: 1.125rem;
		font-weight: 700;
	}

	.empty-state p {
		margin: 0;
		color: var(--text-muted);
		font-size: 0.9375rem;
		line-height: 1.5;
	}

	.empty-state-actions {
		display: flex;
		flex-wrap: wrap;
		justify-content: center;
		gap: 0.75rem;
		margin-top: 1rem;
	}

	.empty-state-primary,
	.empty-state-secondary {
		border-radius: 0.5rem;
		cursor: pointer;
		font-size: 0.875rem;
		font-weight: 700;
		padding: 0.65rem 1.15rem;
		transition:
			background-color 0.2s ease,
			border-color 0.2s ease,
			color 0.2s ease;
	}

	.empty-state-primary {
		border: 1px solid var(--secondary);
		background-color: var(--secondary);
		color: var(--text);
	}

	.empty-state-primary:hover {
		background-color: var(--secondary-hover);
	}

	.empty-state-primary:focus-visible {
		outline: none;
		box-shadow: 0 0 0 3px color-mix(in srgb, var(--secondary) 24%, transparent);
	}

	.empty-state-secondary {
		border: 1px solid var(--border);
		background-color: transparent;
		color: var(--tertiary);
	}

	.empty-state-secondary:hover {
		border-color: var(--tertiary);
		color: var(--tertiary-hover);
	}

	.empty-state-secondary:focus-visible {
		outline: 2px solid var(--tertiary);
		outline-offset: 2px;
	}

	@media (max-width: 480px) {
		.empty-state-actions {
			flex-direction: column;
			width: 100%;
		}

		.empty-state-primary,
		.empty-state-secondary {
			width: 100%;
		}
	}
</style>
