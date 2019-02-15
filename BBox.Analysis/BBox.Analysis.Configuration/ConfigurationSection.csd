<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="d0ed9acb-0435-4532-afdd-b5115bc4d562" namespace="BBox.Analysis.Configuration" xmlSchemaNamespace="bBox" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
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
    <configurationSection name="BBoxAnalysisConfiguration" namespace="BBox.Analysis.Configuration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="bBoxAnalysisConfiguration">
      <elementProperties>
        <elementProperty name="Payments" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="payments" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Payments" />
          </type>
        </elementProperty>
        <elementProperty name="InFileDirectory" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="inFileDirectory" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="OutFileDirectory" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="outFileDirectory" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="FileSearchPattern" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="fileSearchPattern" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="ApiBaseAddress" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="apiBaseAddress" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="Payments" namespace="BBox.Analysis.Configuration" xmlItemName="payment" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PaymentElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="PaymentElement" namespace="BBox.Analysis.Configuration" displayName="payment">
      <elementProperties>
        <elementProperty name="ClientCardType" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="clientCardType" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
        <elementProperty name="PaymentTypes" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="paymentTypes" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/PaymentTypes" />
          </type>
        </elementProperty>
        <elementProperty name="ClientType" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="clientType" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="StringElement">
      <attributeProperties>
        <attributeProperty name="value" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="PaymentTypes" namespace="BBox.Analysis.Configuration" xmlItemName="paymentType" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/StringElement" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>