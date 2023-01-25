# Change Log
Alle updates naar main worden hier bijgehouden. 
 
Dit format is gebaseerd op [Keep a Changelog](http://keepachangelog.com/) en dit project houdt zich aan [Semantic Versioning](http://semver.org/).

## [v1.0.0] - 20-01-2023

Dit zijn alle bugs die gefixt zijn n.a.v. de acceptatietest

### Gefixt
- Foutmeldingen zijn vanaf nu in het Nederlands
- De geboortedatum kan niet meer in de toekomst zijn.
- Er kunnen nu meerdere afdelingen toegewezen worden aan een medewerker.
- Validatie voor vele velden is toegevoegd
- Bugs mbt Medewerkersbeschikbaarheid opgelost
- Namen van de toeslagen zijn Nederlands.

## [v0.9.0] - 22-12-2022

Dit zijn alle features die behoren tot milestone 4 en toegevoegd zijn voor de acceptatietest. 
 
### Toegevoegd
- API voor het in- en uitkloksysteem 
- Optie om gewerkte uren te accorderen
- Mogelijkheid om gewijzige uren in te zien
- Mogelijkheid voor een Admin om branches te beheren
 
 
### Gewijzigd
- Rooster gebruiksvriendelijker gemaakt
 
### Gefixt
- Algemene optimalisaties aan de code
- Databaseoptimalisaties middels Entity Framework plus
- Visuele fixes van het rooster
- Erg veel fixes toegevoegd ten behoeve van de codekwaliteit

## [v0.8.0] - 22-12-2022

Dit zijn alle features die behoren tot milestone 2 & 3. 
 
<h1>Toegevoegd
<h2>Milestone 2</h2>

<h3>Manager</h3>
<h3 >Rooster inzien</h3>
<ul>
 <li>
 Weekoverzicht
 </li>
 <li>
 Dagoverzicht
 </li>
 <li>
 Bechikbaarheid werknemer
 </li>
 <li>
 Inzicht ingeplande uren vs ingeklokte uren
 </li>
 <li>
 Rooster tabel filteren
 </li>
 <li>
 Rooster tabel sorteren
 </li>
 <li>
 Rooster printen
 </li>
 </ul>
 
 <h5 >Rooster beheren</h5>
 <ul>
 <li>
 Nieuwe shift aanmaken (dubbel klik)
 </li>
 <li>
 shift verplatsen (shift slepen)<br>
 LET OP: hoewel dit op zowel de dagoverzichtspagina als de weerkoverzichtspagina mogelijk is, heb je de meeste precisie op de dagoverzichtspagina
 </li>
 <li>
 Shift verkorten/verlengen (rand van shift slepen)
 </li>
 </ul>

<h3>Medewerker</h3>
 <ul>
 <li>
 Rooster inzien
 </li>
 <li>
 Ruilverzoeken aanvragen
 </li>
 </ul>


 <h5>CAO regels</h5>
 <p>De code voor de Nederlandse CAO regels is af, maar nog niet visueel te zien.</p>


<h2>Milestone 3</h2>

<h3>Manager</h3>
<h5>Algemeen</h5>
<ul>
    <li>Globalization/localization toegevoegd voor vertaling medewerkerapplicatie naar Duits</li>
    <li>Applicatie afgesplitst in manager- en medewerkergedeelte</li>
    <li>Notificatiesysteem voor manager</li>
</ul>

<br/>
<h5>Rooster</h5>
<ul>
    <li>Cao/beschikbaarheid-conflicten nu visueel gemaakt in dagscherm</li>
    <li>Inplannen per afdeling nu mogelijk</li>
    <li>Contextmenu met rechtermuisknop, zoals het veranderen van afdeling</li>
    <li>Ziek/beter melden van shifts</li>
    <li>Geklokte uren (verleden) aanpassen en toevoegen</li>
    <li>Werkende grafieken toegevoegd a.d.h.v. correcte data</li>
</ul>

<h5>Overige functies manager</h5>
<ul>
    <li>Gebruikersbeheer toegevoegd voor het aanmaken/verwijderen/aanpassen van medewerkers</li>
    <li>Mogelijkheid toegevoegd om medewerkers te importeren door middel van een csv-bestand</li>
    <li>Exporteren van gewerkte uren per maand mogelijk. (Incl. excelbestand)</li>
</ul>

<h3>Medewerker
<h5>Algemeen</h5>
<li>Globalization/localization toegevoegd voor vertaling medewerkerapplicatie naar Duits</li>
<li>Applicatie afgesplitst in manager- en medewerkergedeelte</li>

<h5>Functies medewerker</h5>
<li>Geoptimaliseerde UI voor mobiele gebruikers</li>
<li>Gewerkte uren inzien</li>
<li>Accountbeheer toegevoegd, voor uitloggen en instellingen</li>
 
<h1>Gewijzigd</h1>
N.v.t.
 
<h1>Gefixt</h1>
N.v.t.
 
## [v0.2.0] - 25-11-2022

Dit zijn alle features die behoren tot milestone 1. 
 
### Toegevoegd
<h4>Identity</h4>
<ul>
    <li>De basisimplementatie van Identity is volledig ingebouwd in het systeem:</li>
    <li>Er kan uitgelogd en ingelogd worden</li>
    <li>Het account kan gemanaged worden, bijvoorbeeld telefoonnummer, e-mail en wachtwoord aanpassen</li>
    <li>Momenteel is er geen onderscheid tussen medewerkers en managers, waardoor iedereen een account kan aanmaken.</li>
    <li>Er is een standaardaccount gemaakt met e-mail <code>manager@bumbo.nl</code> en wachtwoord <code>bumbo</code>.</li>
</ul>

<h4>Historie</h4>
<ul>
    <li>Bekijken van historische gegevens (Aantal klanten en colli)</li>
    <li>Aanpassen van de historische gegevens</li>
</ul>

<h4>Normeringen</h4>
<ul>
    <li>Bekijken van huidige normeringen</li>
    <li>Aanpassen van normeringen</li>
    <li>Bekijken van voorgaande normeringen</li>
</ul>

<h4>Prognose</h4>
<ul>
    <li>Prognosedemopagina waarbij je de datum kan meegeven en er een prognose wordt gemaakt.</li>
    <li>De prognose is gebaseerd op de laatste 4 dagen (Als het op zondag is, dan bijvoorbeeld de laatste 4 zondagen)</li>
    <li>De prognose wordt gemaakt op basis van de historische gegevens (klanten en colli verwacht)</li>
    <li>De prognose wordt gemaakt op basis van de normeringen</li>
    <li>Mocht er binnen 5 dagen een feestdag zijn van de datum die je selecteert, dan wordt er gecheckt of er een feestdag is. Als dit zo is, dan pakt hij de data van het jaar daarvoor, naast de recente data.</li>
</ul>

<h4>Vervangingsverzoeken</h4>
<p>Aangezien er nog geen duidelijke scheiding is tussen de medewerkerapplicatie en de managerapplicatie, is er gekozen voor twee aparte tabs.</p>

<h5>Vervangingsverzoeken manager</h5>
<ul>
    <li>De manager kan vervangingsverzoeken inzien en accepteren</li>
    <li>Er worden enkel vervangingsverzoeken weergegeven die al geaccepteerd zijn door een medewerker</li>
</ul>

<h5>Vervangingsverzoeken medewerker</h5>
<ul>
    <li>Medewerkers kunnen vervangingsverzoeken van zichzelf inzien</li>
    <li>Medewerkers kunnen vervangingsverzoeken van anderen accepteren, onder voorwaarden:</li>
    <li>
        <ul>
            <li>De medewerker behoort tot hetzelfde filiaal als de medewerker die vervangen moet worden</li>
            <li>De medewerker moet op dezelfde afdeling werken als de afdeling van de shift die vervangen moet worden.</li>
            <li>De medewerker werkt nog niet op het moment van de shift die vervangen moet worden.</li>
            <li>Een verzoek kan niet meer geaccepteerd worden een uur voordat deze begint</li>
            <li>De shift is reeds gepubliceerd door de manager</li>
            <li>Het vervangingsverzoek is nog niet ingevuld/geaccepteerd door een andere medewerker.</li>
        </ul>
    </li>
</ul>

<h4>Seed data</h4>
<p>Om de applicatie uitgebreid te kunnen testen, is er seed data gegenereerd. Seed data vult de data met 'nep' data, om zo een goed inzicht te krijgen hoe de applicatie in werking gaat. Er is voor de volgende modellen seed data gegenereerd:</p>
<ul>
    <li>addresses</li>
    <li>departments</li>
    <li>branches</li>
    <li>employees</li>
    <li>availabilities voor employees</li>
    <li>openinghours</li>
    <li>shifts voor employees</li>
    <li>payouts voor employees</li>
    <li>exchangeRequests</li>
    <li>schoolhours</li>
    <li>workstandards</li>
    <li>historical data (for the prognosis)</li>
</ul>
<p>Deze seed data zorgt er voor dat de applicatie bruikbaar en test klaar is.</p>
 
### Gewijzigd
N.v.t.
 
### Gefixt
N.v.t.
