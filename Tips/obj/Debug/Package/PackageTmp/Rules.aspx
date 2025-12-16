<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Rules.aspx.cs" Inherits="VMTips_2018.Rules" %>
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
            Kategori 1: 1 p för rätt tecken, 1 p för rätt resultat, totalt 96 p.<br />
            Kategori 2: 1 p per rätt lag vidare. 1 p bonus om båda rätt. 1 p per rätt placering, totalt 40 p.<br />
            Kategori 3-5: 2 p per rätt, totalt 28 p. <br />
            Kategori 6-9: 4 p per rätt, totalt 16 p.  <br />  
            Totalpoäng hela tipset: 180 poäng.
        </p>

        <p>Skyttekung: Endast kandidater som finns i VM-tipsets databas kan väljas. Avgörande för skyttekung är endast antalet mål.</p>

        <p>
            Utslagsfråga: Vid lika poäng fungerar de olika kategorierna som utslagsfrågor. Den som har flest poäng i kategori 9 vinner. 
            Har båda samma poäng där, vinner den som har flest poäng i kategori 8 och så fortsätter det till avgörande skett i någon 
            av kategorierna. Skulle det inte vara avgjort efter att  kategori 1 räknats, fortsätter utslagsförfarandet match för 
            match i tipsraden, med början nerifrån. Är det fortfarande inte avgjort tillgrips lottning.
        </p>

        <p>Vinnaren tar allt inklusive en pokal.</p>
        
        <p>I och med inlämnandet godkänns att tipskupongen i sin helhet publiceras på Internet.</p>        
    </div>
    
</asp:Content>
