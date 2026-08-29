<script lang="ts">
    import { resolve } from '$app/paths';	
</script>

<!-- FOOTER -->
<footer class="footer">
	<div class="footer-content">
		<div class="footer-brand">
			<div class="footer-logo">
				<span class="logo-estud">Estud</span><span class="logo-aki">aki</span>
			</div>

			<p>
				Estude. Pratique. Conquiste.
			</p>
		</div>

		<div class="footer-links">
			<div>
				<span>Estude</span>
				<a href={resolve('/questions')}>Questões</a>
				<a href={resolve('/questions')}>Vestibulares</a>
				<a href={resolve('/questions')}>Concursos</a>
				<a href={resolve('/questions')}>OAB</a>
			</div>

			<div>
				<span>EstudaKi</span>
				<a href={resolve('/about')}>Sobre</a>
				<a href={resolve('/contact')}>Contato</a>
			</div>
		</div>
	</div>

	<div class="footer-bottom">
		<span>© {new Date().getFullYear()} EstudaKi. Todos os direitos reservados.</span>
	</div>
</footer>

<style>
	:global(body) {
		overflow-x: hidden;        
	}	

	.footer {
		background: var(--surface-elevated);
		border-top: 1px solid var(--border);
	}

	.footer-content {
		max-width: 1440px;
		margin: 0 auto;
		padding: 25px 24px;
		display: flex;
		justify-content: space-between;
		gap: 60px;
	}

	.footer-logo {
		font-size: 1.5rem;
		font-weight: 800;
	}

	.logo-estud {
		color: var(--primary);
	}

	.logo-aki {
		color: var(--secondary);
	}

	.footer-brand p {
		margin-top: 8px;
		color: var(--text-muted);
		font-size: 0.85rem;
	}

	.footer-links {
		display: flex;
		gap: 90px;
	}

	.footer-links div {
		display: flex;
		flex-direction: column;
		gap: 10px;
	}

	.footer-links span {
		margin-bottom: 5px;
		color: var(--text);
		font-size: 0.8rem;
		font-weight: 800;
		text-transform: uppercase;
		letter-spacing: 0.08em;
	}

	.footer-links a {
		color: var(--text-muted);
		font-size: 0.85rem;
		text-decoration: none;
	}

	.footer-links a:hover {
		color: var(--primary);
	}

	.footer-bottom {
		max-width: 1440px;
		margin: 0 auto;
		padding: 20px 24px;
		border-top: 1px solid var(--border);
		color: var(--text-muted);
		font-size: 0.75rem;
	}	

	@media (max-width: 640px) {
		.footer-content {
			flex-direction: column;
		}

		.footer-links {
			gap: 50px;
		}
	}
</style>
