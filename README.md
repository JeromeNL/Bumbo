# Project Bumbo (Semester 3)
During the 'Bumbo' project, a supermarket management system was built in a group of 7 students using ASP.NET MVC

## GitHub workflow
### Commits
1. Maak nooit een directe commit naar `main` of `develop`.
2. Commit zo vaak mogelijk (het liefst nadat je klaar bent met changes aan een bepaalde file) lokaal naar je eigen branch.
3. Zorg dat elke commit naar github een individuele, complete wijziging bevat. Als je bijvoorbeeld een variabele wil hernoemen en een test wil toevoegen, geef ze dan allebei een aparte commit. Hierdoor is het mogelijk om te reverten.
4. Commit naar je github branch als je een pull request wil aanmaken. 
5. Zorg ervoor dat je altijd een commit title, volgens de regels hieronder, toevoegt.
6. Zorg ervoor dat je altijd een commit message toevoegt. Deze omschrijft uitgebreider wat de verandering is, bijvoorbeeld welke methodes je aanmaakt/aanpast. Ook dien je hierin de jira issue te vermelden, bijvoorbeeld PDA-45. 
7. Indien je merget naar main, voeg dan als pull request title een versienummer toe. Voeg alle wijzigingen toe aan [de changelog](https://github.com/Lamineslot/bumbo-3SOd/blob/main/CHANGELOG.md).
8. Zorg ervoor dat je altijd de files waarin je werkt format voordat je commit, om te voorkomen dat Github de hele pagina als wijziging ziet.
8. Volg in onduidelijke gevallen [de GitHub Workflow](https://docs.github.com/en/get-started/quickstart/github-flow) of overleg met Lamine.

#### Commit title
1. Voeg altijd een duidelijke commit title toe. Gebruik hiervoor de onderstaande regels.
2. Maak gebruik van het format `type(onderdeel): <samenvatting van commit>`
   - Verschillende opties voor het type: 
     - `feat`: (new feature for the user, not a new feature for build script)
     - `fix`: (bug fix for the user, not a fix to a build script)
     - `docs`: (changes to the documentation)
     - `style`: (formatting, missing semi colons, etc; no production code change)
     - `refactor`: (refactoring production code, eg. renaming a variable)
     - `test`: (adding missing tests, refactoring tests; no production code change)
     - `chore`: (updating grunt tasks etc; no production code change) 
3. Schrijf de samenvatting in de tegenwoordige tijd, bijvoorbeeld "toevoeging van ClockIn methode".
4. Voorbeeld van een goede commit title: `feat(ClockSystem): toevoeging van ClockIn methode`.

### Branches
1. Maak een eigen branch aan om te werken aan een bepaalde feature. Volg hierbij het format "develop-voornaam-feature", bijvoorbeeld `develop-lamine-roosteritemtoevoegen`. 
2. Verander de naam van je branch niet.
3. Verwijder je branch als de pullrequest is afgehandeld.

## Reviews
1. Elke pull request van develop naar main moet door twee verschillende personen gereviewed worden.
2. Elke pull request van je eigen branch naar develop moet uitvoerig inhoudelijk gereviewed en getest worden.
3. Reviews mag je zelf toewijzen als je weet dat iemand tijd heeft. Anders wijst Lamine reviews toe. 
4. Controleer bij een inhoudelijke review op stijl, indenting, errors, logica en mogelijkheid tot runnen.
5. Geef bij een review feedback op het product/de code en niet de persoon.
6. Als je schrijver van de code bent en je code niet werkt, geef dit dan aan door middel van comments, zodat de PR-reviewer hier rekening mee kan houden.
7. Merge conflicts dien je als indienaar van de code zelf op te lossen. 
8. Indien je niet weet hoe je iets moet oplossen als reviewer, overleg dan met Lamine.
  
## Pull requests
1. Geef pull requests een duidelijke naam, dus niet de naam van github. Indien mogelijk volg je het format voor commit title.
2. Volg het pull request template.
3. Merge een pull request alleen naar main nadat alle comments van reviews zijn verwerkt. 
4. Voeg nadat de pull request is gemerged de issue toe van de hoofdtaak waar je mee bezig bent.

## Applicaties
1. Maak gebruik van GitHub Desktop of de Visual Studio Github interface, tenzij je zeker weer hoe je moet werken met andere systemen. 

## Taal
1. Namen van variabelen, code, comments en dergelijke zijn altijd in het Engels geschreven.
2. Namen van commits en pull requests zijn ook standaard in het Engels.
