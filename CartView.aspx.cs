using System;
using System.Configuration;
using System.Data.OleDb;

public partial class CartView : System.Web.UI.Page
{
    protected CartHelper cartHelper;
    protected int m_CartCount;
    protected int m_SelectedList;
    protected string m_Cmd;
    private OleDbConnection conn;
    protected int m_UserID;
    protected string m_Msg;
    protected int m_WishListsCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        cartHelper = new CartHelper();
        m_CartCount = cartHelper.CartCount();
        m_WishListsCount = 0;

        Utilities.SetLastLoginURL();

        m_SelectedList = Utilities.IForm("SELECTEDLIST");
        
        m_Msg = "";
        
        m_UserID = 0;
        if (Session["UserID"]!=null)
             m_UserID = (Int32)Session["UserID"];

        conn = new OleDbConnection(ConfigurationManager.AppSettings["connString"]);
        conn.Open();

        m_Cmd = Utilities.SForm("CMD");
        if (m_Cmd == "SaveCartAsWishList")
        {
            // save cart items into a new Wish List
            string NewName = Utilities.SForm("WISHLISTNAME");
            int NewWishListID = SaveCartAsNewWishList(NewName);
            if (NewWishListID>0)
            {
                m_Msg = "Your cart items have been saved into your new wish list \"" + NewName + "\".";
            }
            m_SelectedList = NewWishListID;
        }
        else if (m_Cmd == "CopyCartToWishList")
        {
            // copy cart items into an existing Wish List
            int Selected = Utilities.IForm("CMBSELECTED");
            if (CopyCartToWishList(Selected))
            {
                m_Msg = "Your cart items have been copied into your existing wish list \"" + GetWishListName(Selected) + "\".";
            }
            m_SelectedList = Selected;
        }
        else if (m_Cmd == "DeleteWishList" && m_SelectedList>0 && m_UserID>0)
        {
            // delete selected Wish List
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"DELETE CartWishList WHERE UserID=? AND WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", m_UserID);
            cmd.Parameters.AddWithValue("@WishListID", m_SelectedList);
            cmd.ExecuteNonQuery();
            m_SelectedList = 0;
        }
        else if (m_Cmd == "RenameWishList")
        {
            // rename the wish list
            string NewName = Utilities.SForm("WISHLISTNAME");
            if (NewName!="")
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                cmd.CommandText = @"SELECT COUNT(*) FROM CartWishList WHERE UserID=? AND WishListName=?";
                cmd.Parameters.Clear();                
                cmd.Parameters.AddWithValue("@UserID", m_UserID);
                cmd.Parameters.AddWithValue("@WishListName", NewName);
                int Count = Convert.ToInt32(cmd.ExecuteScalar());

                if (Count == 0)
                {
                    cmd.CommandText = @"UPDATE CartWishList SET WishListName=?, DateLastModified=? WHERE UserID=? AND WishListID=?";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@WishListName", NewName);
                    cmd.Parameters.AddWithValue("@DateLastModified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UserID", m_UserID);
                    cmd.Parameters.AddWithValue("@WishListID", m_SelectedList);
                    cmd.ExecuteNonQuery();
                    m_Msg = "Your Wish List was successfuly renamed to \"" + NewName + "\".";
                }
                else
                {
                    m_Msg = "The Wish List name \"" + NewName + "\" already exists, please select a different name.";
                }
            }
        }
        else if (m_Cmd == "CopyWishListToCart")
        {
            // copy wish list items into the cart
            if (m_UserID > 0 && m_SelectedList > 0)
            {            
                if (CopyWishListToCart(m_SelectedList))
                {
                    m_Msg = "Your Wish List items were successfuly copied into your Shopping cart.";
                    m_CartCount = cartHelper.CartCount();
                    m_SelectedList = 0;
                }               
            }            
        }
        else if (m_Cmd == "ClearCart")
        {
            cartHelper.Clear();
            m_SelectedList = 0;
            m_CartCount = 0;
        }
        else if (m_Cmd == "UpdateAndSave")
        {
            if (m_SelectedList==0)
            {
                CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
                if (myCartItems != null)
                {
                    CartItem cartItem;
                    for (int i = myCartItems.Count; i > 0; i--)
                    {
                        cartItem = myCartItems[i - 1];
                        if (Request.Form["Q_" + cartItem.ID] != null)
                        {
                            if (Request.Form["Q_" + cartItem.ID] == "0")
                                myCartItems.Remove(cartItem);
                            else
                                cartItem.Quantity = Convert.ToInt16(Utilities.SForm("Q_" + cartItem.ID));
                        }
                    }
                }
                // Save session cart
                Session["CartItems"] = myCartItems;
            }
            else if (m_SelectedList > 0)
            {
                CartItemCollection myWishListItems = GetWishListItems(m_SelectedList);

                if (myWishListItems != null)
                {

                    CartItem wishListItem;
                    for (int i = myWishListItems.Count; i > 0; i--)
                    {
                        wishListItem = myWishListItems[i - 1];

                        cartHelper.GetPartDetails(ref wishListItem);  // this will update the pricing to the most up to date one!!!
                        
                        if (Request.Form["Q_" + wishListItem.ID] != null)
                        {
                            if (Request.Form["Q_" + wishListItem.ID] == "0")
                                myWishListItems.Remove(wishListItem);
                            else
                                wishListItem.Quantity = Convert.ToInt16(Utilities.SForm("Q_" + wishListItem.ID));
                        }
                    }
                }

                SaveWishListItems(m_SelectedList, myWishListItems);
            }
        }

        m_WishListsCount = GetWishListsCount();
    }


    protected void Page_Unload(object sender, EventArgs e)
    {
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
        }
    }

    private CartItemCollection GetWishListItems(int WishListID)
    {
        CartItemCollection myWishListItems = new CartItemCollection();

        // fill the collection with items from the db
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;

        cmd.CommandText = @"SELECT * FROM CartWishListDetails WHERE WishListID=?";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@WishListID", WishListID);
        dr = cmd.ExecuteReader();

        CartHelper cartHelper = new CartHelper();

        while (dr.Read())
        {
            CartItem wishItem = new CartItem();
            wishItem.PartNumber = dr["PartNumber"].ToString();
            wishItem.Quantity = Convert.ToInt32(dr["Qty"]);
            wishItem.Description = dr["Description"].ToString();
            wishItem.Price = Convert.ToDouble(dr["UnitPrice"]);
            wishItem.Weight = (dr["Weight"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["Weight"].ToString()));
            wishItem.Dimensions = (dr["Dimensions"] == DBNull.Value ? "" : dr["Dimensions"].ToString());
            wishItem.AdditionalShipping = (dr["AdditionalShipping"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["AdditionalShipping"].ToString()));
            wishItem.CommentsShipping = (dr["CommentsShipping"] == DBNull.Value ? "" : dr["CommentsShipping"].ToString());
            wishItem.IsSpecial = (dr["IsSpecial"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsSpecial"].ToString()));
            wishItem.SpecialsText = (dr["SpecialsText"] == DBNull.Value ? "" : dr["SpecialsText"].ToString());
            wishItem.SpecialsText2 = "";

            cartHelper.GetPartDetails(ref wishItem); 

            myWishListItems.Add(wishItem);
        }
        return myWishListItems;
    }

    private bool CopyWishListToCart(int WishListID)
    {
        if (WishListID > 0)
        {
            CartItemCollection myWishListItems = new CartItemCollection();

            myWishListItems = GetWishListItems(WishListID);

            if (myWishListItems.Count > 0)
            {
                CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
                if (myCartItems == null)
                {
                    Session["CartItems"] = myWishListItems;
                }
                else
                {
                    CartHelper cartHelper = new CartHelper();

                    foreach (CartItem wishItem in myWishListItems)
                    {
                        CartItem wI = wishItem;
                        cartHelper.GetPartDetails(ref wI);  // this will update the pricing to the most up to date one!!!

                        myCartItems.Add(wI);
                    }
                    Session["CartItems"] = myCartItems;
                }
            }
            return true;
        }
        return false;
    }

    private bool SaveWishListItems(int WishListID, CartItemCollection myItems)
    {
        if (WishListID > 0)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            cmd.CommandText = @"DELETE CartWishListDetails WHERE WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@WishListID", WishListID);
            cmd.ExecuteNonQuery();

            foreach (CartItem item in myItems)
            {
                cmd.CommandText = @"INSERT INTO CartWishListDetails (WishListID, PartNumber, Qty, Description, UnitPrice, Weight, Dimensions, AdditionalShipping, CommentsShipping, IsSpecial, SpecialsText) VALUES (?,?,?,?,?,?,?,?,?,?,?)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@WishListID", WishListID);
                cmd.Parameters.AddWithValue("@PartNumber", item.PartNumber);
                cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@UnitPrice", item.Price);
                cmd.Parameters.AddWithValue("@Weight", item.Weight);
                cmd.Parameters.AddWithValue("@Dimensions", item.Dimensions);
                cmd.Parameters.AddWithValue("@AdditionalShipping", item.AdditionalShipping);
                cmd.Parameters.AddWithValue("@CommentsShipping", item.CommentsShipping);
                cmd.Parameters.AddWithValue("@IsSpecial", item.IsSpecial);
                cmd.Parameters.AddWithValue("@SpecialsText", item.SpecialsText);
                cmd.ExecuteNonQuery();
            }

            cmd.CommandText = @"UPDATE CartWishList SET DateLastModified=? WHERE WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DateLastModified", DateTime.Now);
            cmd.Parameters.AddWithValue("@WishListID", WishListID);
            cmd.ExecuteNonQuery();

            return true;
        }
        return false;
    }

    private int SaveCartAsNewWishList(string NewName)
    {
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (m_UserID > 0 && NewName != "" && myCartItems != null && Session["UserEMail"] != null)
        {
            cmd.CommandText = @"INSERT INTO CartWishList (UserID, email, WishListName, DateCreated, DateLastModified, DateLastViewed) VALUES (?,?,?,?,?,?); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", m_UserID);
            cmd.Parameters.AddWithValue("@email", Session["UserEMail"]);
            cmd.Parameters.AddWithValue("@WishListName", NewName);
            cmd.Parameters.AddWithValue("@DateCreated", DateTime.Now);
            cmd.Parameters.AddWithValue("@DateLastModified", DateTime.Now);
            cmd.Parameters.AddWithValue("@DateLastViewed", DateTime.Now);            
            int WishListID = Convert.ToInt32(cmd.ExecuteScalar());

            if (SaveWishListItems(WishListID, myCartItems))
                return WishListID;
            else
                return 0;
        }
        return -1;
    }

    private bool CopyCartToWishList(int SelectedWishList)
    {
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;

        CartItemCollection myCartItems = (CartItemCollection)Session["CartItems"];
        if (SelectedWishList>0 && myCartItems != null)
        {
            foreach (CartItem cartItem in myCartItems)
            {
                cmd.CommandText = @"INSERT INTO CartWishListDetails (WishListID, PartNumber, Qty, Description, UnitPrice, Weight, Dimensions, AdditionalShipping, CommentsShipping, IsSpecial, SpecialsText) VALUES (?,?,?,?,?,?,?,?,?,?,?)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@WishListID", SelectedWishList);
                cmd.Parameters.AddWithValue("@PartNumber", cartItem.PartNumber);
                cmd.Parameters.AddWithValue("@Qty", cartItem.Quantity);
                cmd.Parameters.AddWithValue("@Description", cartItem.Description);
                cmd.Parameters.AddWithValue("@UnitPrice", cartItem.Price);
                cmd.Parameters.AddWithValue("@Weight", cartItem.Weight);
                cmd.Parameters.AddWithValue("@Dimensions", cartItem.Dimensions);
                cmd.Parameters.AddWithValue("@AdditionalShipping", cartItem.AdditionalShipping);
                cmd.Parameters.AddWithValue("@CommentsShipping", cartItem.CommentsShipping);
                cmd.Parameters.AddWithValue("@IsSpecial", cartItem.IsSpecial);
                cmd.Parameters.AddWithValue("@SpecialsText", cartItem.SpecialsText);
                cmd.ExecuteNonQuery();                
            }

            cmd.CommandText = @"UPDATE CartWishList SET DateLastModified=? WHERE WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DateLastModified", DateTime.Now);
            cmd.Parameters.AddWithValue("@WishListID", SelectedWishList);
            cmd.ExecuteNonQuery();
            return true;
        }        
        return false;
    }

    protected string ShowWishListsLinks()
    {
        string txt = "";

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;

        if (m_UserID>0)
        {
            cmd.CommandText = @"SELECT DISTINCT WishListID, WishListName, DateLastModified FROM CartWishList WHERE UserID=? ORDER BY WishListName";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", m_UserID);
            dr = cmd.ExecuteReader();
        
            while (dr.Read())
            {
                txt += @"<tr style=""vertical-align: middle;"">            
                            <td align=""right"">
                                " + (Convert.ToInt32(dr["WishListID"]) == m_SelectedList ? @"<img src=""images/ArrowRight.gif"" alt=""Selected"" />" : @"<img src=""images/view-list.gif"" alt=""View Wish List"" style=""cursor: pointer;"" onclick=""document.getElementById('SELECTEDLIST').value='" + dr["WishListID"].ToString() + @"'; document.getElementById('FORM1').submit();"" />") + @"
                            </td>                                  
                            <td align=""left"" style=""" + (Convert.ToInt32(dr["WishListID"]) == m_SelectedList ? @"font-weight: bold;" : @"font-weight: normal;") + @""">" + dr["WishListName"].ToString() + @"</td>
                            <td>&nbsp;&nbsp;</td>
                            <td style=""color: #555; " + (Convert.ToInt32(dr["WishListID"]) == m_SelectedList ? @"font-weight: bold;" : @"font-weight: normal;") + @""">" + String.Format("{0:MM/dd/yyyy}", dr["DateLastModified"]) + @"</td>
                         </tr>";            
            }
        
            if (txt!="")
                txt = @"<table cellpadding=""1"" cellspacing=""2"" border=""0"" style=""font-size: 10px;"">" + txt + @"</table>";
        }

        return txt;

    }

    protected string ShowWishListsComboBox()
    {        
        string txt = "";
        
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;

        if (m_UserID > 0)
        {
            cmd.CommandText = @"SELECT DISTINCT WishListID, WishListName FROM CartWishList WHERE UserID=? ORDER BY WishListName";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", m_UserID);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                txt += @"<option value=""" + dr["WishListID"].ToString() + @""">" + dr["WishListName"].ToString() + @"</option>";  
            }

            if (txt == "")
            {                
                txt = "<td>&nbsp;</td>";
            }
             
        }
        return txt;
    }

    private int GetWishListsCount()
    {
        int Count = -1;
        if (m_UserID > 0)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;            
            cmd.CommandText = @"SELECT COUNT(*) FROM CartWishList WHERE UserID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", m_UserID);
            Count = Convert.ToInt32(cmd.ExecuteScalar());            
        }
        return Count;
    }

    protected int GetWishListItemsCount(int WishListID)
    {
        int Count = -1;        
        if (m_UserID > 0)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;                
            cmd.CommandText = @"SELECT COUNT(*) FROM CartWishListDetails WHERE WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@WishListID", WishListID);
            Count = Convert.ToInt32(cmd.ExecuteScalar());            
        }
        return Count;
    }

    protected string GetWishListName(int WishListID)
    {
        string txt = "";

        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;

        if (m_UserID > 0 && WishListID>0)
        {
            cmd.CommandText = @"SELECT WishListName FROM CartWishList WHERE UserID=? AND WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UserID", m_UserID);
            cmd.Parameters.AddWithValue("@WishListID", WishListID);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                txt = dr["WishListName"].ToString();
            }
        }
        return txt;
    }

    protected string ShowWishList(int WishListID)
    {
        bool AllowUpdates = true;

        string txt = "";
        string trs = "";
        bool bg_alternate = false;

        string q;       //quantity
        string up;      //unit price
        string tp;      //total price
        string qID;     //quantity ID
        string upID;    //unit price ID
        string tpID;    //total price ID
        string asID;    //additional shipping ID
        string dID;     //dimension ID
        string wID;     //weight ID

        string Description; //temp

        CartItemCollection myWishListItems = new CartItemCollection();
        // fill the collection with items from the db
        OleDbCommand cmd = new OleDbCommand();
        cmd.Connection = conn;
        OleDbDataReader dr;

        if (m_UserID > 0)
        {
            CartHelper cartHelper = new CartHelper();

            cmd.CommandText = @"SELECT * FROM CartWishListDetails WHERE WishListID=?";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@WishListID", WishListID);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                CartItem wishItem = new CartItem();
                wishItem.PartNumber = dr["PartNumber"].ToString();
                wishItem.Quantity = Convert.ToInt32(dr["Qty"]);
                wishItem.Description = dr["Description"].ToString();
                wishItem.Price = Convert.ToDouble(dr["UnitPrice"]);
                wishItem.Weight = (dr["Weight"] == DBNull.Value ? 0.0 : Convert.ToDouble(dr["Weight"].ToString()));
                wishItem.Dimensions = (dr["Dimensions"] == DBNull.Value ? "" : dr["Dimensions"].ToString());
                wishItem.AdditionalShipping = (dr["AdditionalShipping"]==DBNull.Value ? 0.0 : Convert.ToDouble(dr["AdditionalShipping"].ToString()));
                wishItem.CommentsShipping = (dr["CommentsShipping"] == DBNull.Value ? "" : dr["CommentsShipping"].ToString());               
                wishItem.IsSpecial = (dr["IsSpecial"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsSpecial"].ToString()));
                wishItem.SpecialsText = (dr["SpecialsText"] == DBNull.Value ? "" : dr["SpecialsText"].ToString());


                cartHelper.GetPartDetails(ref wishItem); 

                myWishListItems.Add(wishItem);
            }
        }


        if (myWishListItems.Count > 0)
        {
            trs = "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td align=\"center\" style=\"width: 75px; border: solid 1px #666;\">Part Number</td>" +
                        "<td align=\"left\"   style=\"width: 420px; border: solid 1px #666;\">&nbsp;&nbsp;Description</td>" +
                        "<td align=\"center\" style=\"width: 30px; border: solid 1px #666; cursor: help;\" title=\"Weight in Pounds\">lb</td>" +
                        "<td align=\"center\" style=\"width: 70px; border: solid 1px #666; cursor: help;\" title=\"Recomended quantity. Price is for each one.\">Qty<img alt=\"Recomended quantity. Price is for each one.\" src=\"images/infoheader.gif\" /></td>" +
                        "<td align=\"center\" style=\"width: 60px; border: solid 1px #666;\">Each</td>" +
                        "<td align=\"center\" style=\"width: 65px; border: solid 1px #666;\">Total</td>" +
                    "</tr>";

            foreach (CartItem cartItem in myWishListItems)
            {
                qID = "Q_" + cartItem.ID;
                upID = "UP_" + cartItem.ID;
                tpID = "TP_" + cartItem.ID;
                asID = "AS_" + cartItem.ID;
                dID = "DIM_" + cartItem.ID;
                wID = "W_" + cartItem.ID;

                q = "document.getElementById('" + qID + "').value";
                up = "document.getElementById('" + upID + "').value";
                tp = "document.getElementById('" + tpID + "').value";

                Description = cartItem.Description;
                if (cartItem.AdditionalShipping > 0.0)
                    Description += "<br />&nbsp;&nbsp;&nbsp;<label style=\"color: #e00; font-weight: bold;\">Additional Shipping charges of " + string.Format("{0:C}", cartItem.AdditionalShipping) + " each!</label>";
                if (cartItem.Dimensions.Length > 0)
                    Description += "<br />&nbsp;&nbsp;&nbsp;<label style=\"color: #e00; font-weight: bold;\">Size \"" + cartItem.Dimensions + "\" (affects shipping costs)</label>";
                if (cartItem.CommentsShipping.Length > 0)
                    Description += "<br />&nbsp;&nbsp;&nbsp;" + cartItem.CommentsShipping;

                // Current cart items

                trs += "<tr style=\"vertical-align: middle; font-family: Arial; font-size: 11px; background-color: " + (bg_alternate ? "#f9f9f9" : "#ddd") + ";\">" +
                            "<td align=\"left\" style=\"border-left: solid 1px #666666; font-weight: bold;\">&nbsp;" + cartItem.PartNumber + "</td>" +
                            "<td align=\"left\" style=\"font-family: Arial Narrow; border-left: solid 1px #666;\">&nbsp;" + Description + "</td>" +
                            "<td align=\"right\" style=\"border-left: solid 1px #666;\">" + (cartItem.Weight == 0.0 ? "&nbsp;" : string.Format("{0:0.00}", cartItem.Weight)) + "&nbsp;</td>" +
                            "<td align=\"center\" style=\"width: 70px; border-left: solid 1px #666;\">" +
                                (AllowUpdates ? "<img src=\"images/minus.gif\" style=\"cursor: pointer; width: 10px;\" alt=\"Decrease quantity\"" +
                                " onclick=\"g_IsDirty=true; if (" + q + ">0) " + q + "--; " + tp + "=NumToCurrency(CurrencyToNum(" + up + ")*" + q + "); UpdateFields();\">" : "") +
                                "<input name=\"" + qID + "\" id=\"" + qID + "\" value=\"" + cartItem.Quantity + "\" type=\"text\" readonly=\"readonly\" style=\"width: 33px; border: 0px; background-color: transparent; text-align: center; font-family: Arial; font-size: 8pt;\" />" +
                                (AllowUpdates ? "<img src=\"images/plus.gif\" style=\"cursor: pointer; width: 10px;\" alt=\"Increase quantity\"" +
                                " onclick=\"g_IsDirty=true; if (" + q + "<999) " + q + "++; " + tp + "=NumToCurrency(CurrencyToNum(" + up + ")*" + q + "); UpdateFields();\">" : "") +
                            "</td>" +
                            "<td align=\"right\" style=\"border-left: solid 1px #666;\"" +
                            (cartItem.IsSpecial ? " title=\"" + (cartItem.SpecialsText2 == "" ? cartItem.SpecialsText : cartItem.SpecialsText2) + "\"" : "") + ">" +
                                "<input type=\"text\" readonly=\"readonly\" id=\"" + upID + "\" value=\"" + string.Format("{0:C}", cartItem.Price) + "\" style=\"width: 58px; border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt;" + (cartItem.IsSpecial ? "cursor: help; font-weight: bold; color: #e60;" : "") + "\" />" +
                            "</td>" +
                            "<td align=\"right\" style=\"border-left: solid 1px #666;\">" +
                                "<input type=\"text\" readonly=\"readonly\" id=\"" + tpID + "\" value=\"" + string.Format("{0:C}", (cartItem.Price * cartItem.Quantity)) + "\" style=\"width: 63px; border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt;\" />" +
                                "<input type=\"hidden\" id=\"" + asID + "\" value=\"" + string.Format("{0:C}", cartItem.AdditionalShipping) + "\" />" +
                                "<input type=\"hidden\" id=\"" + dID + "\" value=\"" + cartItem.Dimensions + "\" />" +
                                "<input type=\"hidden\" id=\"" + wID + "\" value=\"" + (cartItem.Weight == 0.0 ? "&nbsp;" : string.Format("{0:0.00}", cartItem.Weight)) + "\" />" +
                            "</td>" +
                        "</tr>";

                bg_alternate = !bg_alternate;
            }
        }

        if (trs.Length > 0)
        {
            //****************************************************************
            // SubTotal and Total Weight
            //****************************************************************
            trs += "<tr style=\"font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td colspan=\"3\" align=\"right\">" +
                            "Total Weight:<input size=\"3\" type=\"text\" readonly=\"readonly\" name=\"TotalWeight\" id=\"TotalWeight\" value=\"0.0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />lbs" +
                        "</td>" +
                        "<td colspan=\"3\" align=\"right\">" +
                            "Sub Total:<input size=\"6\" type=\"text\" readonly=\"readonly\" name=\"SubTotal\" id=\"SubTotal\" value=\"$0.0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
                        "</td>" +
                    "</tr>";


            //****************************************************************
            // Additional Shipping
            //****************************************************************
            trs += "<tr id=\"TR_ADDITIONALSHIPPING\" style=\"display: none; font-family: Arial; font-size: 11px; font-weight: bold; background-color: #666; color: #fff;\">" +
                        "<td colspan=\"6\" align=\"right\">" +
                            "Additional Shipping Charges:<input size=\"6\" type=\"text\" readonly=\"readonly\" name=\"AdditionalShipping\" id=\"AdditionalShipping\" value=\"-$0.0\" style=\"border: 0px; background-color: transparent; text-align: right; font-family: Arial; font-size: 8pt; font-weight: bold; color: #fff\" />" +
                        "</td>" +
                    "</tr>";


            txt = "<table cellpadding=\"0\" cellspacing=\"0\" style=\"border: solid 1px #666\">" + trs + "</table>";
        }
        return txt;
    }
}

