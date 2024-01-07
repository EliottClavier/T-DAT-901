from spark.apps.test.config.transform.TransformTestUtils import TransformTestUtils
from functools import wraps


# Decorator method used to manage some operations around the
# test_transform method execution.
def test_transform(method):
    @wraps(method)
    def wrapper(self, *args, **kwargs):
        handle_before_method_execution(self)
        result = method(self, *args, **kwargs)
        handle_after_method_execution(self)
        return result

    return wrapper


def handle_before_method_execution(instance):
    TransformTestUtils.convert_tests_files_to_parquet(instance.spark,
                                                      instance.root_path,
                                                      instance.input_format,
                                                      instance.tests_list,
                                                      instance.test_files_path_names)


def handle_after_method_execution(instance):
    TransformTestUtils.clean_test_directories(
        TransformTestUtils.get_tests_path_to_clean(instance.root_path,
                                                   instance.tests_list,
                                                   instance.test_files_path_names))
