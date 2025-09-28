# Programátorská dokumentace – Aplikace FormAI

**Autor:** Petr Ježek  
**Verze:** 1.0  
**Datum:** 16. 9. 2025

---

## Obsah
1.  [Úvod](#1-úvod)
    1.1. [Cíl práce](#11-cíl-práce)
    1.2. [Použité technologie a knihovny](#12-použité-technologie-a-knihovny)
2.  [Návrh a architektura aplikace](#2-návrh-a-architektura-aplikace)
    2.1. [Princip oddělení UI a logiky](#21-princip-oddělení-ui-a-logiky)
    2.2. [Struktura datových souborů](#22-struktura-datových-souborů)
3.  [Implementace - Popis klíčových modulů](#3-implementace---popis-klíčových-modulů)
    3.1. [Základní a konfigurační soubory](#31-základní-a-konfigurační-soubory)
    3.2. [Datové modely](#32-datové-modely)
    3.3. [Jádro aplikace a služby](#33-jádro-aplikace-a-služby)
    3.4. [Uživatelské rozhraní (Views & Controls)](#34-uživatelské-rozhraní-views--controls)
4.  [Závěr](#4-závěr)
    4.1. [Shrnutí](#41-shrnutí)
    4.2. [Možnosti dalšího rozvoje](#42-možnosti-dalšího-rozvoje)

---

## 1. Úvod

### 1.1. Cíl práce

Cílem práce bylo vytvoření desktopové aplikace **FormAI** pro operační systém Windows. Aplikace slouží jako nástroj pro generování personalizovaných jídelníčků pomocí pokročilého jazykového modelu. Uživatel zadá své biometrické údaje, cíle a stravovací preference, na jejichž základě aplikace komunikuje s externím AI a prezentuje uživateli na míru vytvořený stravovací plán.

### 1.2. Použité technologie a knihovny

*   **Platforma:** .NET Framework / .NET Core
*   **Jazyk:** C#
*   **UI Framework:** Windows Presentation Foundation (WPF)
*   **Externí API:** Google Gemini API (pro generování obsahu)
*   **Serializace dat:** `System.Text.Json` (nativní knihovna .NET)
*   **Markdown:** `Markdig` Markdown processor 


## 2. Návrh a architektura aplikace

### 2.1. Princip oddělení UI a logiky

Aplikace důsledně využívá klíčový princip frameworku WPF, kterým je oddělení vzhledu od business logiky.
*   **Vzhled (View):** Uživatelské rozhraní je definováno v deklarativním jazyce XAML (`.xaml`). Tyto soubory popisují hierarchii a vzhled UI komponent.
*   **Logika (Code-Behind):** Chování, obsluha událostí a datové operace jsou implementovány v C# kódu v souborech (`.xaml.cs`). Propojení je zajištěno direktivou `x:Class` v XAML.

### 2.2. Struktura datových souborů

Perzistence uživatelských dat je řešena pomocí jediného souboru ve formátu JSON.
*   **Název souboru:** `FormAI_UserData.json`
*   **Umístění:** `%APPDATA%` (složka `AppData\Roaming` v profilu uživatele).
*   **Struktura:** Soubor obsahuje serializovaný seznam (pole) všech objektů typu `User`. Každá operace se změnou dat (registrace, uložení profilu) zahrnuje deserializaci celého souboru, modifikaci dat v paměti a následnou serializaci zpět na disk.

## 3. Implementace - Popis klíčových modulů

### 3.1. Základní a konfigurační soubory

*   **`App.xaml` / `.cs`**: Vstupní bod aplikace. Zajišťuje načtení všech globálních `ResourceDictionary` (stylů a barev) a definuje startovací okno (`MainWindow`).
*   **`AppSession.cs`**: Statická třída sloužící jako globální kontejner pro stav přihlášení. Její hlavní zodpovědností je držet instanci aktuálně přihlášeného uživatele (`CurrentUser`), která je dostupná ze všech částí aplikace.

### 3.2. Datové modely

*   **`User.cs`**: Klíčový datový model reprezentující jednoho uživatele. Obsahuje všechny vlastnosti od přihlašovacích údajů (`Email`, `PasswordHash`) přes biometrická data (`Height`, `Weight`, `Goal`) až po preference (`Allergies`, `DislikedFoods`).
*   **`CalendarDay.cs`**: Model reprezentující jeden den v kalendáři. Implementuje rozhraní `INotifyPropertyChanged` pro efektivní databinding a automatickou aktualizaci UI (např. změnu barvy) při změně stavu (např. `HasMealPlan`).

### 3.3. Jádro aplikace a služby

*   **`BaseWindow.cs`**: Abstraktní třída, která slouží jako společný předek pro všechna okna v aplikaci. Implementuje logiku pro bezrámečkové okno (custom chrome), včetně funkcí pro jeho přesun, změnu velikosti a ovládací tlačítka (minimalizace, maximalizace, zavření).
*   **`Functions.cs`**: Statická "utility" třída pro pomocné, znovupoužitelné funkce, které nespadají do zodpovědnosti jiných tříd. Primárně obsahuje logiku pro čtení a zápis souborů s jídelníčky.
*   **`MealGeneration.cs`**: Zajišťuje veškerou komunikaci s externím API Google Gemini. Dynamicky sestavuje textový dotaz (prompt) na základě dat z objektu `User`. Odesílá asynchronní `HTTP POST` požadavek, parsuje JSON odpověď a vrací textový výstup od AI. Obsahuje také základní ošetření chyb.

### 3.4. Uživatelské rozhraní (Views & Controls)

*   **`MainWindow.xaml`**: První zobrazené okno, slouží jako uvítací obrazovka a rozcestník k přihlášení nebo registraci.
*   **`RegisterPage.xaml` / `LoginPage.xaml`**: Okna pro správu autentizace uživatelů.
*   **`ProfilePage.xaml`**: Formulář pro zadání a uložení biometrických dat uživatele.
*   **`PreferencePage.xaml`**: Formulář pro zadání alergií a neoblíbených jídel.
*   **`CalendarPage.xaml`**: Hlavní obrazovka aplikace po přihlášení. Zobrazuje kalendář, umožňuje generování a zobrazení jídelníčků.
*   **`MealPlanPage.xaml`**: Okno pro detailní zobrazení vygenerovaného jídelníčku ve formátu Markdown.
*   **`NavigationSidebar.xaml`**: Znovupoužitelná komponenta (User Control) obsahující navigační menu pro přechod mezi klíčovými stránkami aplikace.

## 4. Závěr

### 4.1. Shrnutí

Aplikace FormAI je ve své současné podobě funkčním prototypem, který úspěšně demonstruje koncept generování personalizovaného obsahu pomocí moderních jazykových modelů v desktopovém prostředí. Aplikace správně generuje jídelníčky.

### 4.2 Zkušenosti
**Negativa:**

- Pro tvoření aplikací v C# jsem si zvolil WPF, protože mi přišlo nejintuitivnější. Rozdělení stránek na .cs (backend)
a .xaml (frontend) mi vyhovovalo, protože se podobá webovému designu. Bohužel tohoto rozhoduntí jsem v průběhu vývoje litoval. Stavění UI v "xamlu" je velmi zdlouhavé a vlastně bylo pouze o kopírovaní.

- Komunikace s API je přes `http request`, místo použítí .NET modulů, protože Gemini žádné nepodporuje (a OpenAi nemá "Free tier" svého API pro studenty). 

**Pozitiva:** 

- Propojení .cs s .xaml je velmi efektivní. Například funkce reagující na kliknutí jsou automaticky generované (v Rideru).

- Struktura projektu je WPF formátem "dána", takže jsem se nemusel starat, zda je dostatečně přehledná.
- Všechny moduly a balíčky, které jsem potřeboval (kromě Gemini), byly jednoduše instalovány do projektu pomocí `NuGet Package Manager`.







### 4.3. Možnosti dalšího rozvoje

*   **Lokální databáze:** Nahrazení jednoho JSON souboru robustnějším řešením, jako je SQLite, by zlepšilo výkon a bezpečnost při práci s velkým množstvím uživatelů.
*   **Pokročilá validace vstupů:** Implementace detailnější validace na straně klienta před odesláním dat.
*   **Cloudová synchronizace:** Možnost synchronizovat uživatelská data a jídelníčky mezi více zařízeními.
*   **Možnost generovaní tréninku:** Možnost generovat trénink pro daný den, v závlisosti na jídelničku uživatele. 
*   **Mazání starých jídeníčků:** Jídelníčky, které by si uživatel neuložil, by byly po uplynutí dne, ke kterému jsou přiřazeny, smazány.  
