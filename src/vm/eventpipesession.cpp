// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "common.h"
#include "eventpipe.h"
#include "eventpipeprovider.h"
#include "eventpipesession.h"

#ifdef FEATURE_PERFTRACING

EventPipeSession::EventPipeSession(
    EventPipeSessionType sessionType,
    unsigned int circularBufferSizeInMB,
    EventPipeProviderConfiguration *pProviders,
    unsigned int numProviders)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    m_sessionType = sessionType;
    m_circularBufferSizeInBytes = circularBufferSizeInMB * 1024 * 1024; // 1MB;
    m_rundownEnabled = false;
    m_pProviderList = new EventPipeSessionProviderList(
        pProviders,
        numProviders);
}

EventPipeSession::~EventPipeSession()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if(m_pProviderList != NULL)
    {
        delete m_pProviderList;
        m_pProviderList = NULL;
    }
}

bool EventPipeSession::IsValid() const
{
    LIMITED_METHOD_CONTRACT;

    if((m_pProviderList == NULL) || (m_pProviderList->IsEmpty()))
    {
        return false;
    }

    return true;
}

void EventPipeSession::AddSessionProvider(EventPipeSessionProvider *pProvider)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    m_pProviderList->AddSessionProvider(pProvider);
}

void EventPipeSession::EnableAllEvents()
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    m_pProviderList->EnableAllEvents();
}

EventPipeSessionProvider* EventPipeSession::GetSessionProvider(EventPipeProvider *pProvider)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    return m_pProviderList->GetSessionProvider(pProvider);
}

EventPipeSessionProviderList::EventPipeSessionProviderList(
    EventPipeProviderConfiguration *pConfigs,
    unsigned int numConfigs)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    m_pProviders = new SList<SListElem<EventPipeSessionProvider*>>();
    m_pCatchAllProvider = NULL;
    for(unsigned int i=0; i<numConfigs; i++)
    {
        EventPipeProviderConfiguration *pConfig = &pConfigs[i];
        EventPipeSessionProvider *pProvider = new EventPipeSessionProvider(
            pConfig->GetProviderName(),
            pConfig->GetKeywords(),
            (EventPipeEventLevel)pConfig->GetLevel());

        m_pProviders->InsertTail(new SListElem<EventPipeSessionProvider*>(pProvider));
    }
}

EventPipeSessionProviderList::~EventPipeSessionProviderList()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if(m_pProviders != NULL)
    {
        SListElem<EventPipeSessionProvider*> *pElem = m_pProviders->GetHead();
        while(pElem != NULL)
        {
            EventPipeSessionProvider *pProvider = pElem->GetValue();
            delete pProvider;

            SListElem<EventPipeSessionProvider*> *pCurElem = pElem;
            pElem = m_pProviders->GetNext(pElem);
            delete pCurElem;
        }

        delete m_pProviders;
        m_pProviders = NULL;
    }
    if(m_pCatchAllProvider != NULL)
    {
        delete(m_pCatchAllProvider);
        m_pCatchAllProvider = NULL;
    }
}

void EventPipeSessionProviderList::AddSessionProvider(EventPipeSessionProvider *pProvider)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if(pProvider != NULL)
    {
        m_pProviders->InsertTail(new SListElem<EventPipeSessionProvider*>(pProvider));
    }
}

void EventPipeSessionProviderList::EnableAllEvents()
{
    LIMITED_METHOD_CONTRACT;

    if(m_pCatchAllProvider == NULL)
    {
        m_pCatchAllProvider = new EventPipeSessionProvider(NULL, 0xFFFFFFFFFFFFFFFF, EventPipeEventLevel::Verbose);
    }
}

EventPipeSessionProvider* EventPipeSessionProviderList::GetSessionProvider(
    EventPipeProvider *pProvider)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    // Exists when tracing was enabled at start-up and all events were requested. This is a diagnostic config.
    if(m_pCatchAllProvider != NULL)
    {
        return m_pCatchAllProvider;
    }

    if(m_pProviders == NULL)
    {
        return NULL;
    }

    SString providerNameStr = pProvider->GetProviderName();
    LPCWSTR providerName = providerNameStr.GetUnicode();

    EventPipeSessionProvider *pSessionProvider = NULL;
    SListElem<EventPipeSessionProvider*> *pElem = m_pProviders->GetHead();
    while(pElem != NULL)
    {
        EventPipeSessionProvider *pCandidate = pElem->GetValue();
        if(wcscmp(providerName, pCandidate->GetProviderName()) == 0)
        {
            pSessionProvider = pCandidate;
            break;
        }
        pElem = m_pProviders->GetNext(pElem);
    }

    return pSessionProvider;
}

bool EventPipeSessionProviderList::IsEmpty() const
{
    LIMITED_METHOD_CONTRACT;

    return (m_pProviders->IsEmpty() && m_pCatchAllProvider == NULL);
}

EventPipeSessionProvider::EventPipeSessionProvider(
    LPCWSTR providerName,
    UINT64 keywords,
    EventPipeEventLevel loggingLevel)
{
    CONTRACTL
    {
        THROWS;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if(providerName != NULL)
    {
        size_t bufSize = wcslen(providerName) + 1;
        m_pProviderName = new WCHAR[bufSize];
        wcscpy_s(m_pProviderName, bufSize, providerName);
    }
    else
    {
        m_pProviderName = NULL;
    }
    m_keywords = keywords;
    m_loggingLevel = loggingLevel;

}

EventPipeSessionProvider::~EventPipeSessionProvider()
{
    CONTRACTL
    {
        NOTHROW;
        GC_NOTRIGGER;
        MODE_ANY;
    }
    CONTRACTL_END;

    if(m_pProviderName != NULL)
    {
        delete[] m_pProviderName;
        m_pProviderName = NULL;
    }
}

LPCWSTR EventPipeSessionProvider::GetProviderName() const
{
    LIMITED_METHOD_CONTRACT;
    return m_pProviderName;
}

UINT64 EventPipeSessionProvider::GetKeywords() const
{
    LIMITED_METHOD_CONTRACT;
    return m_keywords;
}

EventPipeEventLevel EventPipeSessionProvider::GetLevel() const
{
    LIMITED_METHOD_CONTRACT;
    return m_loggingLevel;
}

#endif // FEATURE_PERFTRACING
