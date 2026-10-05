<script lang="ts">
	import { Card } from 'flowbite-svelte';    
	import {
		BookOpenOutline,
		CheckCircleOutline
	} from 'flowbite-svelte-icons';
</script>

<section class="section features-section">
	<div class="section-header">
		<span class="eyebrow">FEITO PARA ESTUDAR</span>

		<h2>
			Menos distração.
			<span>Mais prática.</span>
		</h2>

		<p>
			Tudo o que você precisa para transformar tempo de estudo em prática de
			verdade.
		</p>
	</div>

	<div class="features">
		<Card class="feature-card">
			<div class="feature-icon green-icon">
				<BookOpenOutline />
			</div>

			<h3>Banco de questões</h3>

			<p>
				Encontre questões de diferentes provas e instituições em um único
				lugar.
			</p>
		</Card>

		<Card class="feature-card">
			<div class="feature-icon orange-icon">
				<BookOpenOutline />
			</div>

			<h3>Busca inteligente</h3>

			<p>
				Encontre rapidamente questões por assunto, disciplina, prova ou
				instituição.
			</p>
		</Card>

		<Card class="feature-card">
			<div class="feature-icon purple-icon">
				<CheckCircleOutline />
			</div>

			<h3>Acompanhe sua evolução</h3>

			<p>
				Entenda seu desempenho e descubra quais conteúdos precisam de mais
				atenção.
			</p>
		</Card>
	</div>
</section>

<style>
	:global(body) {
		overflow-x: hidden;        
	}	

	.section {
		max-width: 1440px;
		margin: 0 auto;
		padding: 110px 24px;
	}

	.section-header {
		max-width: 700px;
		margin: 0 auto 55px;
		text-align: center;
	}	

	.section-header h2 {
		margin: 0;
		color: var(--text);
		font-size: clamp(2.2rem, 4vw, 3.6rem);
		line-height: 1.05;
		letter-spacing: -0.04em;
		font-weight: 800;
	}

	.section-header h2 span {
		color: var(--primary);
	}

	.section-header p {
		margin-top: 18px;
		color: var(--text-muted);
		line-height: 1.7;
	}		

	.features {
		display: grid;
		grid-template-columns: repeat(3, 1fr);
		gap: 18px;
	}

	.feature-icon {
		display: grid;
		width: 50px;
		height: 50px;
		place-items: center;
		margin-bottom: 24px;
		border-radius: 14px;
	}

	.green-icon {
		background: color-mix(in srgb, var(--primary) 14%, transparent);
		color: var(--primary);
	}

	.orange-icon {
		background: color-mix(in srgb, var(--secondary) 14%, transparent);
		color: var(--secondary);
	}

	.purple-icon {
		background: color-mix(in srgb, var(--tertiary) 14%, transparent);
		color: var(--tertiary);
	}	
	
</style>
