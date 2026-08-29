<script lang="ts">
    import { resolve } from '$app/paths';
	import {
		BookOpenOutline,
		GraduationCapOutline,
		BriefcaseOutline,
		ArrowRightOutline,
	} from 'flowbite-svelte-icons';
</script>

<section class="section categories-section">
	<div class="categories">
		<a href={resolve('/questions')} class="category-card green">
			<div class="category-icon">
				<GraduationCapOutline />
			</div>

			<div>
				<span class="category-label">VESTIBULARES</span>
				<h3>Entre na universidade</h3>
				<p>
					Pratique questões de vestibulares e prepare-se para as principais
					provas.
				</p>
			</div>

			<ArrowRightOutline class="category-arrow" />
		</a>

		<a href={resolve('/questions')} class="category-card orange">
			<div class="category-icon">
				<BriefcaseOutline />
			</div>

			<div>
				<span class="category-label">CONCURSOS</span>
				<h3>Conquiste sua vaga</h3>
				<p>
					Estude para concursos públicos praticando questões de diferentes
					bancas.
				</p>
			</div>

			<ArrowRightOutline class="category-arrow" />
		</a>

		<a href={resolve('/questions')} class="category-card purple">
			<div class="category-icon">
				<BookOpenOutline />
			</div>

			<div>
				<span class="category-label">OAB</span>
				<h3>Prepare-se para a prova</h3>
				<p>
					Pratique questões da OAB e fortaleça seu conhecimento para o exame.
				</p>
			</div>

			<ArrowRightOutline class="category-arrow" />
		</a>
	</div>
</section>

<style>
    :global(body) {        
		overflow-x: hidden;
	}

    .section {
		padding: 15px 0px 50px;
		margin: 0 20px;
	}

    .categories {
		display: grid;
		grid-template-columns: repeat(3, 1fr);
		gap: 18px;
	}

    
	.category-card {
		position: relative;
		min-height: 280px;
		display: flex;
		flex-direction: column;
		padding: 32px;
		border: 1px solid var(--border);
		border-radius: 24px;
		background: var(--surface);
		text-decoration: none;
		transition:
			transform 0.2s ease,
			box-shadow 0.2s ease;
	}

	.category-card:hover {
		transform: translateY(-5px);
		box-shadow: 0 20px 50px color-mix(in srgb, var(--text) 8%, transparent);
	}

	.category-icon {
		display: grid;
		width: 52px;
		height: 52px;
		place-items: center;
		margin-bottom: 35px;
		border-radius: 15px;
	}

	.green .category-icon {
		background: color-mix(in srgb, var(--primary) 14%, transparent);
		color: var(--primary);
	}

	.orange .category-icon {
		background: color-mix(in srgb, var(--secondary) 14%, transparent);
		color: var(--secondary);
	}

	.purple .category-icon {
		background: color-mix(in srgb, var(--tertiary) 14%, transparent);
		color: var(--tertiary);
	}

	.category-label {
		font-size: 0.7rem;
		font-weight: 800;
		letter-spacing: 0.12em;
		color: var(--text-muted);
	}

	.category-card h3 {
		margin: 7px 0 10px;
		color: var(--text);
		font-size: 1.4rem;
	}

	.category-card p {
		margin: 0;
		color: var(--text-muted);
		line-height: 1.6;
		font-size: 0.9rem;
	}
</style>