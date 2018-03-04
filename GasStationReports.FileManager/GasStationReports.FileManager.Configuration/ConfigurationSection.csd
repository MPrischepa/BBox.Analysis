<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="d0ed9acb-0435-4532-afdd-b5115bc4d562" namespace="GasStationReports.FileManager.Configuration" xmlSchemaNamespace="urn:GasStationReports.FileManager.Configuration" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="FileManagerConfiguration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="fileManagerConfiguration">
      <elementProperties>
        <elementProperty name="InputDirectories" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="inputDirectories" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/InputDirectoryCollection" />
          </type>
        </elementProperty>
        <elementProperty name="RemoveFiles" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="removeFiles" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/RemoveFileCollection" />
          </type>
        </elementProperty>
        <elementProperty name="ArchiveFilePattern" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="archiveFilePattern" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="OutputDirectory" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="outputDirectory" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="ReplacementTextTemplates" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="replacementTextTemplates" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ReplacementTemplateCollection" />
          </type>
        </elementProperty>
        <elementProperty name="AfterExecuteApplications" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="afterExecuteApplications" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ApplicationCollections" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="InputDirectoryCollection" xmlItemName="inputDirectory" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="BaseDirectory" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="baseDirectory" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/InputDirectoryElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="InputDirectoryElement">
      <attributeProperties>
        <attributeProperty name="Directory" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="directory" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="RemoveFileCollection" xmlItemName="removeFile" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/RemoveFile" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="RemoveFile">
      <attributeProperties>
        <attributeProperty name="FillePattern" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="fillePattern" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="StringElement">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="ReplacementTemplateCollection" xmlItemName="replacementTemplateElement" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ReplacementTemplateElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ReplacementTemplateElement">
      <elementProperties>
        <elementProperty name="SearchPattern" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="searchPattern" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="ReplacementPattern" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="replacementPattern" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="TemplateDescription" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="templateDescription" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="Index" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="index" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Index" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="Index">
      <attributeProperties>
        <attributeProperty name="Ind" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="ind" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="ApplicationElement">
      <elementProperties>
        <elementProperty name="FileName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="fileName" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="Arguments" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="arguments" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ArgumentCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="ArgumentCollection" xmlItemName="argument" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElementCollection name="ApplicationCollections" xmlItemName="application" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/ApplicationElement" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>