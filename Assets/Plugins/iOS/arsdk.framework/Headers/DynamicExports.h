#ifndef _DYNAMIC_EXPORTS_H_
#define _DYNAMIC_EXPORTS_H_

#ifndef ARSDK_EXPORTS
#define ARSDK_API
#else
#if OS_ALVA == OS_Windows || OS_ALVA == OS_UWP
#define ARSDK_API __declspec(dllexport)
#endif//OS_ALVA

#if OS_ALVA == OS_Android
#define ARSDK_API __attribute__((visibility("protected")))
#endif//OS_ALVA

#if OS_ALVA == OS_Linux
#define ARSDK_API __attribute__((visibility("default")))
#endif//OS_ALVA

#if OS_ALVA == OS_iOS
#define ARSDK_API
#endif//OS_ALVA
#endif

#endif