#ifndef _SHARED_MACHINE_CODE_INTERFACE_H_
#define _SHARED_MACHINE_CODE_INTERFACE_H_

#include "DynamicExports.h"

#ifdef __cplusplus
extern "C"
{
#endif
    /**
     * @brief 获取机器码
     * @param code 输出, 机器码
     *             用于获取授权
     *             用户管理内存, 最小 41 bytes
     * @return 0: 成功, 其他: 失败
     */
    ARSDK_API int cGetMachineCode(unsigned char* code);
#ifdef __cplusplus
}
#endif

#endif
