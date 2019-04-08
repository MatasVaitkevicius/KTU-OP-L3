<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="L3_Web.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style>
        .Table {
            height: 20rem;
            width: 95rem
        }
        .Label {
            font-size: 1.5rem;
            font-weight: bold;
            font-family: 'Times New Roman';
        }
        .Button {
            height: 5rem;
            width: 15rem;
            background-color:cornsilk;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="LabelLD" runat="server" Text="LD2_16" CssClass="Label"></asp:Label>
            <br />
            <br />
        </div>
        <div>
            <asp:Label ID="LabelAT" runat="server" Text="Pasirinkite 16a.txt" CssClass="Label"></asp:Label>
            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="Button" Height="30px"/>
            <br />
            <br />
        </div>
        <div>
            <asp:Label ID="LabelBT" runat="server" Text="Pasirinkite 16b.txt" CssClass="Label"></asp:Label>
            <asp:FileUpload ID="FileUpload2" runat="server" CssClass="Button" Height="30px" />
            <br />
            <br />

        </div>

        <div>
            <asp:Button ID="Button1" runat="server" Text="Skaityti ir Vykdyti" OnClick="Button1_Click" CssClass="Button" Font-Size="Medium"/>
        </div>
        <div>
            <br />
            <asp:Label ID="LabelU" runat="server" Text="U16a.txt" CssClass="Label"></asp:Label>
            <br />
            <asp:Table ID="Table1" runat="server" CssClass="Table">
            </asp:Table>
        </div>
        <div>
            <br />
            <br />
            <asp:Label ID="LabelR" runat="server" Text="U16b.txt" CssClass="Label"></asp:Label>
            <asp:Table ID="Table2" runat="server" CssClass="Table"></asp:Table>
            <br />
            <br />
        </div>
        <div>
            <asp:Label ID="LabelF" runat="server" Text="Gyventojų sąrašas, kurie už komunalines paslaugas per metus mokėjo sumą, mažesnę už vidutinę, ir surikiuoti pagal pavardę ir vardą abėcėlės tvarka" CssClass="Label"></asp:Label>
            <br />
            <asp:Table ID="Table3" runat="server" CssClass="Table">
            </asp:Table>
        </div>
        <br />
        <br />
        <br />
        <br />
        <br />
        <div>
           <asp:Label ID="LabelM" runat="server" Text="Įveskite mėnesį pagal kurį šalinsite" CssClass="Label"></asp:Label>
            <br />
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            <br />
            <br />
            <br />
            <br />
        </div>
        <div>
            <asp:Label ID="LabelK" runat="server" Text="Įveskite paslauga pagal kurią šalinsite" CssClass="Label"></asp:Label>
            <br />
            <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
            <br />
            <br />
            <br />
            <br />
        </div>
        <div>
            <asp:Button ID="Button2" runat="server" Text="Pašalinti" OnClick="Button2_Click" CssClass="Button" Font-Size="Medium"/>
            <br />
            <br />
            <br />
            <br />
        </div>
        <asp:Label ID="Label4" runat="server" CssClass="Label"></asp:Label>
        <br />
        <div>
            <asp:Table ID="Table4" runat="server" CssClass="Table">
            </asp:Table>
            <br />
            <br />
            <br />
        </div>
    </form>
</body>
</html>
