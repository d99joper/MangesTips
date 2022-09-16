<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Rules.aspx.cs" Inherits="VMTips_2022.Rules" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="wrap">

        <p>Regler:</p>
            
        <p>Deltagande: Tipset är endast öppet för personer som känner Magnus. Endast en kupong per person är tillåten.</p>

        <p>
            Inlämning: Kupongen anses inte inlämnad förrän kupongen blivit godkänd av Magnus och betalning har skett. 
            Deltagaren kommer då att få en slutlig bekräftelse på deltagande och kupongen kommer att synas på hemsidan.
        </p>            
        
        <p>
            Poängsättning: <br />
            Kategori 1: 2 p för rätt tecken, 1 p för rätt resultat, totalt 144 p.<br />
            Kategori 2: 2 p per rätt lag vidare. 2 p bonus om båda rätt. 2 p per rätt placering, totalt 80 p.<br />
            Kategori 3-5: 4 p per rätt, totalt 56 p.<br />
            Kategori 6-9: 10 p per rätt, totalt 40 p.<br />  
            Totalpoäng hela tipset: 320 poäng.
        </p>

        <p>
            Skyttekung: Endast kandidater som finns i VM-tipsets databas kan väljas, testa och skriv in namnet du
            vill tippa, finns det i databasen kommer det att komma upp i lista nedanför. Avgörande för skyttekung
            är endast antalet mål, vilket innebär att poäng kan ges för flera spelare om de gör lika antal mål.</p>

        <p>
            Utslagsfråga: Vid lika poäng fungerar de olika kategorierna som utslagsfrågor. Den som har flest
            poäng i kategori 9 vinner. Har båda samma poäng där, vinner den som har flest poäng i kategori 8 och
            så fortsätter det till avgörande skett i någon av kategorierna. Skulle det inte vara avgjort efter att
            kategori 1 räknats, fortsätter utslagsförfarandet match för match i tipsraden, med början nerifrån. Är
            det fortfarande inte avgjort tillgrips lottning.
        </p>

        <p>
            Vinnaren får halva prispotten inklusive en pokal och får välja ett välgörande ändamål att skänka andra
            halvan av prispotten till. Organisationen ska ha ett aktivt godkänt 90-konto hos Svensk
            Insamlingskontroll, <a href="https://www.insamlingskontroll.se/90konto" target="_blank">https://www.insamlingskontroll.se/90konto</a> .</p>
        
        <p>I och med inlämnandet godkänns att tipskupongen i sin helhet publiceras på Internet.</p>        
    </div>
    
</asp:Content>
