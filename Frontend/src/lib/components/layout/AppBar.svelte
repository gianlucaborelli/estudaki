<script lang="ts">
  import { Navbar, NavBrand, NavUl, NavLi } from "flowbite-svelte";
  import ThemeToggle from "./ThemeToggle.svelte";
  import  logo  from "$lib/assets/icon.png";
</script>

<Navbar class="site-navbar">
  <NavBrand href="/">
    <img class="logo" src={logo} alt="Estudaki logo" />
    <span class="brand">
      <span class="brand-estud">Estud</span><span class="brand-aki">aki</span>
    </span>
  </NavBrand>
  <div class="flex md:order-2">
    <NavUl class="mobile-menu">
      <NavLi href="/" class="nav-item">Início</NavLi>      
      <NavLi href="/questions" class="nav-item">Questões</NavLi>
      <NavLi href="/about" class="nav-item">Sobre</NavLi>
      <NavLi href="/contact" class="nav-item">Contato</NavLi>
    </NavUl>
    <ThemeToggle />
  </div>
</Navbar>

<style>
  .logo {
    width: 40px;
    height: 40px;
    margin-right: 10px;
  }

  :global(.site-navbar) {
    background-color: var(--primary);
  }

  :global(span) {
    font-size: 1.5rem;
  }

  :global(.nav-item) {
    color: var(--text);
    font-size: 1rem;
    margin-right: 20px;    
  }

  :global(.brand) {
    font-size: 1.8rem;
    font-weight: 700;
    white-space: nowrap;
  }

  .brand-estud {
    color: var(--text);
  }

  .brand-aki {
    color: var(--secondary);
  }
</style>