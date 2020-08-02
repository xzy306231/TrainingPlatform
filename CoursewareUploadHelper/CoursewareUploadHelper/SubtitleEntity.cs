
// 注意: 生成的代码可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class organizations
{

    private organizationsOrganization organizationField;

    /// <remarks/>
    public organizationsOrganization organization
    {
        get => this.organizationField;
        set => this.organizationField = value;
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class organizationsOrganization
{

    private string titleField;

    private organizationsOrganizationMetadata metadataField;

    private organizationsOrganizationItem[] itemField;

    private string idField;

    /// <remarks/>
    public string title
    {
        get => this.titleField;
        set => this.titleField = value;
    }

    /// <remarks/>
    public organizationsOrganizationMetadata metadata
    {
        get => this.metadataField;
        set => this.metadataField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("item")]
    public organizationsOrganizationItem[] item
    {
        get => this.itemField;
        set => this.itemField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
        get => this.idField;
        set => this.idField = value;
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class organizationsOrganizationMetadata
{

    private string languageField;

    private object descriptionField;

    private object keywordsField;

    private System.DateTime dateField;

    private object durationField;

    private object versionField;

    private object generatorField;

    private object package_idField;

    private object rightsField;

    private string content_folderField;

    /// <remarks/>
    public string language
    {
        get => this.languageField;
        set => this.languageField = value;
    }

    /// <remarks/>
    public object description
    {
        get => this.descriptionField;
        set => this.descriptionField = value;
    }

    /// <remarks/>
    public object keywords
    {
        get => this.keywordsField;
        set => this.keywordsField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
    public System.DateTime date
    {
        get => this.dateField;
        set => this.dateField = value;
    }

    /// <remarks/>
    public object duration
    {
        get => this.durationField;
        set => this.durationField = value;
    }

    /// <remarks/>
    public object version
    {
        get => this.versionField;
        set => this.versionField = value;
    }

    /// <remarks/>
    public object generator
    {
        get => this.generatorField;
        set => this.generatorField = value;
    }

    /// <remarks/>
    public object package_id
    {
        get => this.package_idField;
        set => this.package_idField = value;
    }

    /// <remarks/>
    public object rights
    {
        get => this.rightsField;
        set => this.rightsField = value;
    }

    /// <remarks/>
    public string content_folder
    {
        get => this.content_folderField;
        set => this.content_folderField = value;
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class organizationsOrganizationItem
{

    private string titleField;

    private organizationsOrganizationItemItem[] itemField;

    private string idField;

    private string locationField;

    private string itemtypeField;

    private string completionField;

    private bool auto_forwardField;

    private bool visibleField;

    private string nav_buttonField;

    /// <remarks/>
    public string title
    {
        get => this.titleField;
        set => this.titleField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("item")]
    public organizationsOrganizationItemItem[] item
    {
        get => this.itemField;
        set => this.itemField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
        get => this.idField;
        set => this.idField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string location
    {
        get => this.locationField;
        set => this.locationField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string itemtype
    {
        get => this.itemtypeField;
        set => this.itemtypeField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string completion
    {
        get => this.completionField;
        set => this.completionField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool auto_forward
    {
        get => this.auto_forwardField;
        set => this.auto_forwardField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool visible
    {
        get => this.visibleField;
        set => this.visibleField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nav_button
    {
        get => this.nav_buttonField;
        set => this.nav_buttonField = value;
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class organizationsOrganizationItemItem
{

    private string titleField;

    private string idField;

    private string locationField;

    private string itemtypeField;

    private string completionField;

    private bool auto_forwardField;

    private bool visibleField;

    private string nav_buttonField;

    /// <remarks/>
    public string title
    {
        get => this.titleField;
        set => this.titleField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
        get => this.idField;
        set => this.idField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string location
    {
        get => this.locationField;
        set => this.locationField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string itemtype
    {
        get => this.itemtypeField;
        set => this.itemtypeField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string completion
    {
        get => this.completionField;
        set => this.completionField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool auto_forward
    {
        get => this.auto_forwardField;
        set => this.auto_forwardField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool visible
    {
        get => this.visibleField;
        set => this.visibleField = value;
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nav_button
    {
        get => this.nav_buttonField;
        set => this.nav_buttonField = value;
    }
}

